local mg      = require "moongen"
local memory  = require "memory"
local device  = require "device"
local ts      = require "timestamping"
local stats   = require "stats"
local log     = require "log"
local limiter = require "software-ratecontrol"
local pipe    = require "pipe"
local ffi     = require "ffi"
local libmoon = require "libmoon"
local histogram = require "histogram"
--local bit64   = require "bit64"

local namespaces = require "namespaces"
local socket = require("socket")


local PKT_SIZE	= 60

function configure(parser)
	parser:description("Forward traffic between interfaces with moongen rate control")
	parser:option("-d --dev", "Devices to use, specify the same device twice to echo packets."):args(2):convert(tonumber)
	parser:option("-r --rate", "Forwarding rates in Mbps (two values for two links)"):args(2):convert(tonumber)
	parser:option("-t --threads", "Number of threads per forwarding direction using RSS."):args(1):convert(tonumber):default(1)
	parser:option("-l --latency", "Fixed emulated latency (in ms) on the link."):args(2):convert(tonumber):default({0,0})
	parser:option("-q --queuedepth", "Maximum number of packets to hold in the delay line"):args(2):convert(tonumber):default({0,0})
	parser:option("-o --loss", "Rate of packet drops"):args(2):convert(tonumber):default({0,0})
	parser:option("-H --harq", "HARQ loss rate on the link."):args(2):convert(tonumber):default({0,0})
	return parser:parse()
end


function master(args)
	-- configure devices
	for i, dev in ipairs(args.dev) do
		args.dev[i] = device.config{
			port = dev,
			txQueues = args.threads,
			rxQueues = args.threads,
			rssQueues = 0,
			rssFunctions = {},
			--rxDescs = 4096,
			dropEnable = true,
			disableOffloads = true
		}
	end
	device.waitForLinks()

	-- print stats
	stats.startStatsTask{devices = args.dev}
	
	-- create the ring buffers
	-- should set the size here, based on the line speed and latency, and maybe desired queue depth
	local qdepth1 = args.queuedepth[1]
	qdepth1 = math.ceil(2097152/1280)


	if qdepth1 < 1 then
		qdepth1 = math.ceil((args.latency[1] * args.rate[1] * 1000)/672)
		if (qdepth1 == 0) then
			qdepth1 = 1
		end

		print("automatically setting qdepth1="..qdepth1)
	end
	local qdepth2 = args.queuedepth[2]
	if qdepth2 < 1 then
		qdepth2 = math.ceil((args.latency[2] * args.rate[2] * 1000)/672)
		if (qdepth2 == 0) then
			qdepth2 = 1
		end
		print("automatically setting qdepth2="..qdepth2)
	end
	local ring1 = pipe:newPktsizedRing(qdepth1)
	local ring2 = pipe:newPktsizedRing(qdepth2)



	local ns = namespaces:get()



	-- start the forwarding tasks
	for i = 1, args.threads do
		mg.startTask("forward", ring1, args.dev[1]:getTxQueue(i - 1), args.dev[1], ns, args.rate[1], args.latency[1], args.loss[1], args.harq[1],1)
		if args.dev[1] ~= args.dev[2] then
			mg.startTask("forward", ring2, args.dev[2]:getTxQueue(i - 1), args.dev[2], ns, args.rate[2], args.latency[2], args.loss[2], args.harq[2],2)
		end
	end

	-- start the receiving/latency tasks
	for i = 1, args.threads do
		mg.startTask("receive", ring1, args.dev[2]:getRxQueue(i - 1), args.dev[2])
		if args.dev[1] ~= args.dev[2] then
			mg.startTask("receive", ring2, args.dev[1]:getRxQueue(i - 1), args.dev[1])
		end
	end

	--mg.startTask("server", ns)

	mg.waitForTasks()
end


function receive(ring, rxQueue, rxDev)
	--print("receive thread...")

	local bufs = memory.createBufArray()
	local count = 0
	local count_hist = histogram:new()
	local ringsize_hist = histogram:new()
	local ringbytes_hist = histogram:new()
	while mg.running() do
		count = rxQueue:recv(bufs)
		count_hist:update(count)
		--print("receive thread count="..count)
		for iix=1,count do
			local buf = bufs[iix]
			local ts = limiter:get_tsc_cycles()
			buf.udata64 = ts
			--print("RXRX arrival: ", bit64.tohex(buf.udata64))
		end
		if count > 0 then
			pipe:sendToPktsizedRing(ring.ring, bufs, count)
			--print("ring count: ",pipe:countPacketRing(ring.ring))
			ringsize_hist:update(pipe:countPktsizedRing(ring.ring))
		end
	end
	count_hist:print()
	count_hist:save("rxq-pkt-count-distribution-histogram-"..rxDev["id"]..".csv")
	ringsize_hist:print()
	ringsize_hist:save("rxq-ringsize-distribution-histogram-"..rxDev["id"]..".csv")
end

function forward(ring, txQueue, txDev, ns, rate, latency, lossrate, harqLossRate, threadId)

	local latencyHarq = 6;
	local harqMaxAttempt = 4;

	print("forward with rate "..rate.." and latency "..latency.." and loss rate "..lossrate)
	local numThreads = 1
	
	local linkspeed = txDev:getLinkStatus().speed
	print("linkspeed = "..linkspeed)

	local tsc_hz = libmoon:getCyclesFrequency()
	local tsc_hz_ms = tsc_hz / 1000
	print("tsc_hz = "..tsc_hz)

	local packetInfo = {}
	local packetInfoLength = 0

	ns.messageToSend = nil
	ns.messageId = 1
	ns.packetInfoLength = 0

	-- larger batch size is useful when sending it through a rate limiter
	local bufs = memory.createBufArray()  --memory:bufArray()  --(128)
	local count = 0

	local lastPrintTime = 0
	local last_send_time = 0;

	while mg.running() do

		-- receive one or more packets from the queue
		count = pipe:recvFromPktsizedRing(ring.ring, bufs, 1)
		local sendCount = count

		for iix=1,count do

			harqLossRate = ns.HARQ_loss_rate or 0
	 		throughPutPdcp = ns.PDCP_throughput or rate

			local currentTime = os.time()

			if currentTime - lastPrintTime >= 1 then
				--print("Updated HARQ Loss Rate: " .. harqLossRate)
				lastPrintTime = currentTime  
			end



			local retransmissionAttempt = 0;
			local buf = bufs[iix]
			
			-- get the buf's arrival timestamp and compute departure time
			while math.random() < harqLossRate and retransmissionAttempt <= harqMaxAttempt do
				retransmissionAttempt = retransmissionAttempt + 1
			end


			if retransmissionAttempt > harqMaxAttempt then
				loss = true;
				sendCount = sendCount - 1
				print("HARQ retransmission max attempts reached, packet lost")
				retransmissionAttempt = 4
			else
				--print("HARQ retransmission attempt: ", retransmissionAttempt)
			end



			-- local current_time = limiter:get_tsc_cycles()
			-- get the buf's arrival timestamp and compute departure time
			local arrival_timestamp = buf.udata64
			--print("TXTX arrival: ", bit64.tohex(buf.udata64))


			--local send_time = arrival_timestamp + (latencyHarq * tsc_hz_ms * retransmissionAttempt) + latency * tsc_hz_ms

			--local min_latency_due_to_throughput = (1292 * 8) / throughPutPdcp * tsc_hz_ms

			local min_latency_due_to_throughput = 1 / 67000000 * tsc_hz

			local send_time = arrival_timestamp + (latencyHarq * tsc_hz_ms * retransmissionAttempt)
			local send_time_limit = last_send_time + min_latency_due_to_throughput

			if send_time_limit > send_time then
				send_time = send_time_limit
			end

			last_send_time = send_time


			--print("TXTX send time: ", bit64.tohex(send_time))
			 --print("TXTX tsc_cycles: ", bit64.tohex(limiter:get_tsc_cycles()))

			-- spin/wait until it is time to send this frame
			-- this does not allow reordering of frames
			while limiter:get_tsc_cycles() < send_time do
				if not mg.running() then
					return
				end
			end

			
			if threadId == 2 then
			local packetId = packetInfoLength + iix
            packetInfo[packetId] = {
                id = packetId,
                receiveTime = buf.udata64 / tsc_hz_ms,
                sendTime = 0
            }
			end

			local pktSize = buf.pkt_len + 24
			--print("TXTX set delay: ", (pktSize) * (linkspeed/rate - 1))
			buf:setDelay((pktSize) * (linkspeed/rate - 1))


		end

		if count > 0 then
			-- the rate here doesn't affect the result afaict.  It's just to help decide the size of the bad pkts
			txQueue:sendWithDelayLoss(bufs, rate * numThreads, lossrate, sendCount)

			local currentSendTime = limiter:get_tsc_cycles() / tsc_hz_ms

			if threadId == 2 then
			for iix = 1, count do

				sendCount = sendCount - 1

				if sendCount < 0 then
					currentSendTime = "Lost"
				end

                local packetId = packetInfoLength + iix
                if packetInfo[packetId] then
                    packetInfo[packetId].sendTime = currentSendTime 
					
					--print(string.format("Inserted Packet Info: ID=%d, ReceiveTime=%s, SendTime=%s",
					--	packetInfo[packetId].id,
					--	tostring(packetInfo[packetId].receiveTime),
					--	tostring(packetInfo[packetId].sendTime)
					--))

                end
            end
			end

		end

		packetInfoLength = packetInfoLength + count
		ns.packetInfoLength = packetInfoLength



if packetInfo[ns.packetIdToSend] and ns.packetIdToSend < packetInfoLength then
    local packetIdToSend = ns.packetIdToSend
    local messageTable = {} 

    local maxBatchSize = 100  
    local batchCount = 0

    while packetIdToSend < packetInfoLength and batchCount < maxBatchSize do
        local packet = packetInfo[packetIdToSend]
        if packet and packet.id and packet.receiveTime and packet.sendTime then
            table.insert(messageTable, 
                string.format("PktId: %d, RX: %s, TX: %s", 
                packet.id, tostring(packet.receiveTime), tostring(packet.sendTime))
            )
            ns.messageId = packet.id
            packetIdToSend = packetIdToSend + 1
            batchCount = batchCount + 1
        else
            print("Packet missing required fields for ID: " .. tostring(ns.packetIdToSend))
        end
    end

    ns.messageToSend = table.concat(messageTable, "\n")  
else
    -- print("No packet found for ID: " .. tostring(ns.packetIdToSend))
end



	end
end


function server(ns)

	local tsc_hz = libmoon:getCyclesFrequency()
	local tsc_hz_ms = tsc_hz / 1000

    local server = assert(socket.bind("127.0.0.1", 12345))
    server:settimeout(0)
    print("Server listening on 127.0.0.1:12345")


	ns.packetIdToSend = 1

    while mg.running() do
        local client = server:accept()
        if client then
			client:settimeout(0)
            local message, err = client:receive()
            if not err and message then
                print("Received raw message:", message)
                local ok, data = pcall(load("return " .. message))
                if ok and type(data) == "table" then
                    --print("Parsed table:", data)
					--print("Old HARQ_loss_rate:", ns.HARQ_loss_rate)
					ns.HARQ_loss_rate = data.HARQ_loss_rate
					--print("New HARQ_loss_rate:", ns.HARQ_loss_rate)
					ns.PDCP_throughput = data.PDCP_throughput
					print("New PDCP_throughput:", ns.PDCP_throughput)
                else
                    print("Invalid Lua table format.")
                end
                client:send("Processed: " .. message .. "\n")

            else
                print("Receive error:", err)
            end

            client:close()
        end

		mg.sleepMillis(1)  

		while ns.packetInfoLength and ns.packetIdToSend <= ns.packetInfoLength do
			local success, test_err = pcall(function()
			if ns.messageToSend and ns.messageId >= ns.packetIdToSend then
				local send_client = socket.tcp()
				send_client:settimeout(0.1)

				if send_client:connect("127.0.0.1", 12350) then
					send_client:send(ns.messageToSend )
					ns.packetIdToSend = ns.messageId + 1	
				end
				
				send_client:close()
			else
				 --print("Packet with ID " .. ns.packetIdToSend .. " not found.")
			end



			--print("Sent message to 127.0.0.1:12350:", ns.messageToSend)

			end)

			if not success then
			    --print("Failed to send test message to 127.0.0.1:12350. Error:", test_err)
			end

			mg.sleepMillis(1)  

		end

    end

    server:close()
    print("Server shut down.")
end


