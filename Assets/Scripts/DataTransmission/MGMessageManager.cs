using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static MGData;

public class MGMessageManager : MonoBehaviour
{
    private List<pkt> pktInfo = new List<pkt>();
    private List<rlcQueue> rlcQueueInfo = new List<rlcQueue>();
    private List<rlcLoss> rlcLossInfo = new List<rlcLoss>();
    private List<pdcpThroughput> pdcpThroughputInfo = new List<pdcpThroughput>();
    private List<latencyTotal> latencyTotalInfo = new List<latencyTotal>();


    private int queueMaxTemp;

    public List<pkt> getPktInfo () { return pktInfo; }
    public List<rlcQueue> getRlcQueueInfo () {  return rlcQueueInfo; }
    public List<rlcLoss> getRlcLossInfo () { return rlcLossInfo; }
    public List<pdcpThroughput> GetPdcpThroughputs () 
    { 
        CalculateThroughputPerSecond();
        return pdcpThroughputInfo; 
    }

    public List <latencyTotal> GetLatencyTotalInfo () 
    {  
        CalculateLatencyPerSecond();
        return latencyTotalInfo; 
    }


    public void HandlePktMessage(string message)
    {
        Regex pktRegex = new Regex(@"\[Pkt\] Id: (\d+), RX: (\d+)ULL, TX: (\d+)ULL, RLCQ: (\d+)");
        Match match = pktRegex.Match(message);

        if (match.Success)
        {
            pkt newPkt = new pkt
            {
                id = int.Parse(match.Groups[1].Value),
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

            rlcQueueInfo.Add(newRlc);
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

            rlcQueueInfo.Add(newRlc);
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


    public void CalculateThroughputPerSecond()
    {
        var sortedPkts = pktInfo
            .Where(pkt => pkt.txTime != "Loss")
            .OrderBy(pkt => int.Parse(pkt.txTime))
            .ToList();

        Dictionary<int, int> throughputData = new Dictionary<int, int>();

        foreach (var pkt in sortedPkts)
        {
            if (int.TryParse(pkt.txTime, out int txTimeValue))
            {
                int currentSecond = txTimeValue / 1000;        

                if (!throughputData.ContainsKey(currentSecond))
                {
                    throughputData[currentSecond] = 0;
                }
                throughputData[currentSecond]++;

            }
        }

        pdcpThroughputInfo.Clear();
        foreach (var entry in throughputData)
        {
            pdcpThroughputInfo.Add(new pdcpThroughput
            {
                time = entry.Key.ToString(),
                throughput = entry.Value
            });
        }
    }


    public void CalculateLatencyPerSecond()
    {
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
                int currentSecond = txTimeValue / 1000;
                int latency = txTimeValue - rxTimeValue; // µ¥Î»Îªms

                if (!latencyData.ContainsKey(currentSecond))
                {
                    latencyData[currentSecond] = (0, 0);
                }

                var currentData = latencyData[currentSecond];
                latencyData[currentSecond] = (currentData.totalLatency + latency, currentData.count + 1);
            }
        }

        latencyTotalInfo.Clear();
        foreach (var entry in latencyData)
        {
            double avgLatencySeconds = (double)entry.Value.totalLatency / entry.Value.count / 1000.0;
            latencyTotalInfo.Add(new latencyTotal
            {
                time = entry.Key.ToString(),
                latency = avgLatencySeconds.ToString("F3")
            });
        }
    }


}
