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
local preFile = ""

function configure(parser)
	parser:description("Forward traffic between interfaces with moongen rate control")
	parser:option("-d --dev", "Devices to use, specify the same device twice to echo packets."):args(2):convert(tonumber)
	parser:option("-r --rate", "Forwarding rates in Mbps (two values for two links)"):args(2):convert(tonumber)
	parser:option("-t --threads", "Number of threads per forwarding direction using RSS."):args(1):convert(tonumber):default(1)
	parser:option("-l --latency", "Fixed emulated latency (in ms) on the link."):args(2):convert(tonumber):default({0,0})
	parser:option("-q --queuedepth", "Maximum number of packets to hold in the delay line"):args(2):convert(tonumber):default({0,0})
	parser:option("-o --loss", "Rate of packet drops"):args(2):convert(tonumber):default({0,0})
	parser:option("-H --harq", "HARQ loss rate on the link."):args(2):convert(tonumber):default({0,0})

	parser:flag("-s --use_listener", "Enable listener.")
    parser:flag("-f --read_file", "Read preconfigured file.")


	return parser:parse()
end


function master(args)

	if args.read_file then
    
		local filepath = "./examples/emulation_data_record.lua"
		print("Attempting to load file from:", filepath)
    
		preFile = loadDataFromFile(filepath)
		preFile = stringToTable (preFile)
		
		print(preFile)
    
		if preFile then
		    print("File loaded successfully.")
		else
		    print("Failed to load file.")
		end
	end

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
	--qdepth1 = math.ceil(2097152/1280)


	if qdepth1 < 1 then
		qdepth1 = math.ceil((args.latency[1] * args.rate[1] * 1000)/672)
		if (qdepth1 == 0) then
			qdepth1 = 1
		end

		print("automatically setting qdepth1="..qdepth1)
	end
	local qdepth2 = args.queuedepth[2]


	--qdepth2 = math.ceil(2097152/1280)


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
		mg.startTask("forward", ring1, args.dev[1]:getTxQueue(i - 1), args.dev[1], ns, args.rate[1], args.latency[1], args.loss[1], args.harq[1], 1, args.use_listener, args.read_file, preFile)
		if args.dev[1] ~= args.dev[2] then
			mg.startTask("forward", ring2, args.dev[2]:getTxQueue(i - 1), args.dev[2], ns, args.rate[2], args.latency[2], args.loss[2], args.harq[2], 2, args.use_listener, args.read_file, preFile)
		end
	end

	-- start the receiving/latency tasks
	for i = 1, args.threads do
		mg.startTask("receive", ring1, args.dev[2]:getRxQueue(i - 1), args.dev[2], ns, 1)
		if args.dev[1] ~= args.dev[2] then
			mg.startTask("receive", ring2, args.dev[1]:getRxQueue(i - 1), args.dev[1], ns, 2)
		end
	end


	mg.startTask("server", ns, args.use_listener)
	

	mg.waitForTasks()
end


function receive(ring, rxQueue, rxDev, ns, threadId)
	--print("receive thread...")

	local bufs = memory.createBufArray()
	local count = 0
	local count_hist = histogram:new()
	local ringsize_hist = histogram:new()
	local ringbytes_hist = histogram:new()

	local tsc_hz = libmoon:getCyclesFrequency()
	local tsc_hz_ms = tsc_hz / 1000
	local overflow_count = 0
	local start_time = limiter:get_tsc_cycles() / tsc_hz_ms
	--local ring_capacity = pipe:capacityPktsizedRing(ring.ring)
	--local ring_capacity = math.ceil(2097152/1280)
	local ring_capacity = math.ceil(2048000/(1250*1.14))


	--ring_capacity = 20

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
			local ring_count = pipe:countPktsizedRing(ring.ring)
			if ring_count + count <= ring_capacity then
				pipe:sendToPktsizedRing(ring.ring, bufs, count)
			else 
				overflow_count = overflow_count + count
				bufs:free(count)
			end
			--print("ring count: ",pipe:countPktsizedRing(ring.ring))
			ringsize_hist:update(pipe:countPktsizedRing(ring.ring))
		end

		if threadId == 1 then
			local current_time = limiter:get_tsc_cycles() / tsc_hz_ms
			ns.rlcMessage = string.format("[RLC] T: %s, Loss: %d, Queue: %d, of %d\n", 
                                              tostring(current_time - start_time), overflow_count or 0, pipe:countPktsizedRing(ring.ring) or 0, ring_capacity)
			--print("RLC Loss: ",overflow_count)
		end

		

	end
	count_hist:print()
	count_hist:save("rxq-pkt-count-distribution-histogram-"..rxDev["id"]..".csv")
	ringsize_hist:print()
	ringsize_hist:save("rxq-ringsize-distribution-histogram-"..rxDev["id"]..".csv")
end

function forward(ring, txQueue, txDev, ns, rate, latency, lossrate, harqLossRate, threadId, listener, file, preFile)


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
	local last_send_time = 0
	local last_file_read_time = 0
	local emu_time = 0
	local start_time = limiter:get_tsc_cycles() / tsc_hz_ms

	local packetBuffer = {}
	local bufferIndex = 1
	local throughput = 0
	
	local baseLatency = 0
	--local extraLatency = 16

	local harqLossRate = 0
	local latencyHarq = 8
	local harqLossReduction = 0.2
	local harqMaxAttempt = 4


	while mg.running() do

		-- receive one or more packets from the queue
		count = pipe:recvFromPktsizedRing(ring.ring, bufs, 1)
		local sendCount = count
		local dequeue_timestamp = limiter:get_tsc_cycles() / tsc_hz_ms

		for iix=1,count do

			if listener then
				harqLossRate = ns.HARQ_loss_rate or 0
	 			throughput = ns.PDCP_throughput or rate
			end

			if file then
				emu_time = (limiter:get_tsc_cycles() / tsc_hz_ms - start_time)
				if (emu_time - last_file_read_time) > 100 then
					throughput = getThroughput(emu_time, preFile)
					harqLossRate = getPer(emu_time, preFile)
					last_file_read_time = emu_time
					-- print("emu_time: ", emu_time)
				end
			end


			local retransmissionAttempt = 0;
			local buf = bufs[iix]

			local harqLossRateReduced =  harqLossRate







			harqLossRateReduced = 0










			-- print("harqLossRate: ", harqLossRate)

			-- get the buf's arrival timestamp and compute departure time
			while math.random() < harqLossRateReduced and retransmissionAttempt <= harqMaxAttempt do
				retransmissionAttempt = retransmissionAttempt + 1
				harqLossRateReduced =  harqLossRate * harqLossReduction ^ retransmissionAttempt
			end


			if retransmissionAttempt > harqMaxAttempt then
				loss = true;
				sendCount = sendCount - 1
				print("HARQ retransmission max attempts reached, packet lost")
				retransmissionAttempt = 4
			else
				-- print("HARQ retransmission attempt: ", retransmissionAttempt)
			end

			-- local current_time = limiter:get_tsc_cycles()
			-- get the buf's arrival timestamp and compute departure time
			local arrival_timestamp = buf.udata64
			--print("TXTX arrival: ", bit64.tohex(buf.udata64))


			--local send_time = arrival_timestamp + (latencyHarq * tsc_hz_ms * retransmissionAttempt) + latency * tsc_hz_ms

			--local min_latency_due_to_throughput = (1292 * 8) / throughPutPdcp * tsc_hz_ms



			local min_latency_due_to_throughput = 1 / (throughput) * tsc_hz

			if last_send_time == 0 then
				last_send_time = arrival_timestamp
			end

			local send_time_base = arrival_timestamp + baseLatency * tsc_hz_ms + retransmissionAttempt * (latencyHarq) * tsc_hz_ms
			local send_time_limit = last_send_time + retransmissionAttempt * (latencyHarq) * tsc_hz_ms + min_latency_due_to_throughput
			
			if send_time_base > send_time_limit then
				send_time = send_time_base
			else
				send_time = send_time_limit
			end

			last_send_time = send_time
			--send_time = send_time + extraLatency * tsc_hz_ms

			--print("TXTX send time: ", bit64.tohex(send_time))
			 --print("TXTX tsc_cycles: ", bit64.tohex(limiter:get_tsc_cycles()))

			-- spin/wait until it is time to send this frame
			-- this does not allow reordering of frames
			while limiter:get_tsc_cycles() < send_time do
				if not mg.running() then
					return
				end
			end

			
			if threadId == 1 then
			local packetId = packetInfoLength + iix
            packetInfo[packetId] = {
                id = packetId,

                receiveTime = buf.udata64 / tsc_hz_ms - start_time,

				-- receiveTime = dequeue_timestamp - start_time,

                sendTime = 0,
				rlcQueue = pipe:countPktsizedRing(ring.ring)
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

			if threadId == 1 then
			for iix = 1, count do

				sendCount = sendCount - 1

				if sendCount < 0 then
					currentSendTime = "Lost"
				end


                local packetId = packetInfoLength + iix
                if packetInfo[packetId] and sendCount >= 0 then
                    packetInfo[packetId].sendTime = currentSendTime  - start_time
					
					--print(string.format("Inserted Packet Info: ID=%d, ReceiveTime=%s, SendTime=%s",
					--	packetInfo[packetId].id,
					--	tostring(packetInfo[packetId].receiveTime),
					--	tostring(packetInfo[packetId].sendTime)
					--))
				else if sendCount < 0 then
					packetInfo[packetId].sendTime = "Lost"
				end
                end
            end
			end
			
		end

		packetInfoLength = packetInfoLength + count
		ns.packetInfoLength = packetInfoLength

		local currentTime = limiter:get_tsc_cycles() / tsc_hz



if currentTime - lastPrintTime >= 0.1 and packetInfo[ns.packetIdToSend] and ns.packetIdToSend < packetInfoLength and ns.packetIdToSend >= ns.messageId then
    local packetIdToSend = ns.packetIdToSend

    local maxBatchSize = 1000  
    local batchCount = 0

    while packetIdToSend < packetInfoLength and batchCount < maxBatchSize do
        local packet = packetInfo[packetIdToSend]
        if packet and packet.id and packet.receiveTime and packet.sendTime then
            packetBuffer[bufferIndex] = 
                string.format("[Pkt] Id: %d, RX: %s, TX: %s, RLCQ: %d\n", 
                packet.id, tostring(packet.receiveTime), tostring(packet.sendTime), packet.rlcQueue)

            ns.messageId = packet.id
            packetIdToSend = packetIdToSend + 1
            bufferIndex = bufferIndex + 1
            batchCount = batchCount + 1
        else
            print("Packet missing required fields for ID: " .. tostring(ns.packetIdToSend))
        end
    end

    lastPrintTime = currentTime  

    if bufferIndex > 1 then
        ns.messageToSend = table.concat(packetBuffer, "")  
        packetBuffer = {}  
        bufferIndex = 1
    end
end



	end
end


function server(ns, listener)

	local tsc_hz = libmoon:getCyclesFrequency()
	local tsc_hz_ms = tsc_hz / 1000

    local server = assert(socket.bind("127.0.0.1", 12345))
    server:settimeout(0)
    print("Server listening on 127.0.0.1:12345")

	local lastReportTime = limiter:get_tsc_cycles() / tsc_hz_ms
	local startTime = limiter:get_tsc_cycles() / tsc_hz_ms


	ns.packetIdToSend = 1

    while mg.running() do
		if listener then
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
        end

		mg.sleepMillis(1)  

		
	local success, test_err = pcall(function()
		local send_client = socket.tcp()
        send_client:settimeout(0.1)
        while ns.packetInfoLength and ns.packetIdToSend <= ns.packetInfoLength do
            if ns.messageToSend and ns.messageId >= ns.packetIdToSend then
                if send_client:connect("127.0.0.1", 12350) then
                    send_client:send(ns.messageToSend)
                    ns.packetIdToSend = ns.messageId + 1    
                end        
            end
        end
		send_client:close()
    end)

    if not success then
        print("Error sending message: ", test_err)
    end

    local currentTime = limiter:get_tsc_cycles() / tsc_hz_ms
    if currentTime > lastReportTime + 1000 then
        local report_client = socket.tcp()
        report_client:settimeout(0.1)

        if report_client:connect("127.0.0.1", 12350) and ns.rlcMessage then
            report_client:send(ns.rlcMessage)
        end

        report_client:close()
        lastReportTime = currentTime
    end

		mg.sleepMillis(1)
	end
    server:close()
    print("Server shut down.")
end

function loadDataFromFile(filepath)
    local file = io.open(filepath, "r") 
    if not file then
        return nil
    end
    local content = file:read("*a") 
    return content
end

function getThroughput(current_time, data)

--print("Data before calling getThroughput:", data) 

if not data then
    print("Error: data is nil!") 
else
    --print("Data length:", #data) 
end

    for i = #data, 1, -1 do 
        local entry_time = tonumber(data[i].t) * 1000
        if current_time > entry_time then
			print("Current throughput: ", data[i].throughput)
            return data[i].throughput
        end
    end
    return 0 
end

function getPer(current_time, data)


if not data then
    print("Error: data is nil!") 
else
    --print("Data length:", #data) 
end

    for i = #data, 1, -1 do 
        local entry_time = tonumber(data[i].t) * 1000
        if current_time > entry_time then
			print("Current PER: ", data[i].per)
            return data[i].per
        end
    end
    return 0 
end

function stringToTable(str)
    local func = load("return " .. str) 
    if func then
        local ok, result = pcall(func) 
        if ok then
            return result
        else
            print("Error parsing string:", result)
        end
    end
    return nil
end


