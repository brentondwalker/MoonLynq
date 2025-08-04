using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static MGData;

public class MGMessageManager : MonoBehaviour
{
    private List<pkt> pktInfo = new List<pkt>();
    private List<rlcQueue> rlcQueueBase = new List<rlcQueue>();
    private List<rlcLoss> rlcLossInfo = new List<rlcLoss>();
    private List<pdcpThroughput> pdcpThroughputInfo = new List<pdcpThroughput>();
    private List<latencyTotal> latencyTotalInfo = new List<latencyTotal>();


    private int queueMaxTemp;

    public List<pkt> getPktInfo () { return pktInfo; }
    public List<rlcQueue> getRlcQueueBase () {  return rlcQueueBase; }
    public List<rlcLoss> getRlcLossInfo () { return rlcLossInfo; }
    public List<pdcpThroughput> GetPdcpThroughputs () 
    { 
        CalculateThroughputPerInterval();
        return pdcpThroughputInfo; 
    }

    public List <latencyTotal> GetLatencyTotalInfo () 
    {  
        CalculateLatencyByInterval();
        return latencyTotalInfo; 
    }


    public void HandlePktMessage(string message)
    {
        Regex pktRegex = new Regex(@"\[Pkt\] Id: (\d+), RX: (\d+)ULL, TX: (\d+)ULL, RLCQ: (\d+)");
        Match match = pktRegex.Match(message);

        if (match.Success)
        {
            int pktId = int.Parse(match.Groups[1].Value);

            bool pktExists = pktInfo.Any(pkt => pkt.id == pktId);
            if (!pktExists)
            {
                pkt newPkt = new pkt
                {
                    id = pktId,
                    rxTime = match.Groups[2].Value,
                    txTime = match.Groups[3].Value
                };

                pktInfo.Add(newPkt);
                //Debug.Log($"[Pkt] Parsed: Id={newPkt.id}, RX={newPkt.rxTime}, TX={newPkt.txTime}");

                rlcQueue newRlc = new rlcQueue
                {
                    time = match.Groups[3].Value,
                    queue = int.Parse(match.Groups[4].Value)
                };

                rlcQueueBase.Add(newRlc);
            }
            else
            {
                //Debug.Log($"Duplicate Pkt Id detected: {pktId}");
            }
        }
        else
        {
            Debug.LogError($"Failed to parse Pkt message: {message}");
        }
    }

    public void HandleRlcMessage(string message)
    {
        Regex rlcRegex = new Regex(@"\[RLC\] T: (\d+)ULL, Loss: (\d+), Queue: (\d+), of (\d+)");
        Match match = rlcRegex.Match(message);

        if (match.Success)
        {
            rlcQueue newRlc = new rlcQueue
            {
                time = match.Groups[1].Value,
                queue = int.Parse(match.Groups[3].Value),
            };

            queueMaxTemp = int.Parse(match.Groups[4].Value);

            rlcQueueBase.Add(newRlc);
            //Debug.Log($"[RLC] Parsed: T={newRlc.time}, Loss={newRlc.loss}, Queue={newRlc.queue}/{newRlc.queueMax}");

            rlcLoss newRlcLoss = new rlcLoss
            {
                time = match.Groups[1].Value,
                loss = int.Parse(match.Groups[2].Value),
            };

            rlcLossInfo.Add(newRlcLoss);
        }
        else
        {
            Debug.LogError($"Failed to parse RLC message: {message}");
        }
    }


    public void CalculateThroughputPerInterval()
    {
        var sortedPkts = pktInfo
            .Where(pkt => pkt.txTime != "Loss")
            .OrderBy(pkt => int.Parse(pkt.txTime))
            .ToList();

        int interval = 100;

        if (sortedPkts.Count == 0) return;
        int minTimeSlot = int.Parse(sortedPkts.First().txTime) / interval;
        int maxTimeSlot = int.Parse(sortedPkts.Last().txTime) / interval;

        Dictionary<int, int> throughputData = new Dictionary<int, int>();

        for (int t = minTimeSlot; t <= maxTimeSlot; t++)
        {
            throughputData[t] = 0;  
        }

        foreach (var pkt in sortedPkts)
        {
            if (int.TryParse(pkt.txTime, out int txTimeValue))
            {
                int timeSlot = txTimeValue / interval;
                throughputData[timeSlot]++; 
            }
        }

        pdcpThroughputInfo.Clear();
        foreach (var entry in throughputData)
        {
            pdcpThroughputInfo.Add(new pdcpThroughput
            {
                time = (entry.Key * interval).ToString(),  
                throughput = entry.Value
            });
        }
    }


    public void CalculateLatencyByInterval()
    {
        int interval = 100;


        var sortedPkts = pktInfo
            .Where(pkt => pkt.txTime != "Loss")
            .OrderBy(pkt => int.Parse(pkt.txTime))
            .ToList();

        Dictionary<int, (int totalLatency, int count)> latencyData = new Dictionary<int, (int, int)>();

        foreach (var pkt in sortedPkts)
        {
            if (int.TryParse(pkt.txTime, out int txTimeValue) &&
                int.TryParse(pkt.rxTime, out int rxTimeValue))
            {
                int intervalKey = txTimeValue / interval;
                int latency = txTimeValue - rxTimeValue;

                if (!latencyData.ContainsKey(intervalKey))
                {
                    latencyData[intervalKey] = (0, 0);
                }

                var currentData = latencyData[intervalKey];
                latencyData[intervalKey] = (currentData.totalLatency + latency, currentData.count + 1);
            }
        }

        latencyTotalInfo.Clear();
        foreach (var entry in latencyData)
        {
            double avgLatencySeconds = (double)entry.Value.totalLatency / entry.Value.count / 1000.0;
            double intervalStartSec = (entry.Key * interval) / 1000.0;

            latencyTotalInfo.Add(new latencyTotal
            {
                time = intervalStartSec.ToString("F3"),
                latency = avgLatencySeconds.ToString("F3")
            });
        }
    }


}
