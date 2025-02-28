using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using static MGData;

public class MGDataToJSON : MonoBehaviour
{
    public MGMessageManager messageManager;

    public void SaveAllJson()
    {
        ThroughputJson();
        LatencyJson();
        PktJson();
    }

    public void ThroughputJson()
    {
        List<pdcpThroughput> pdcpData = messageManager.GetPdcpThroughputs();

        if (pdcpData == null || pdcpData.Count == 0)
        {
            Debug.LogWarning("No PDCP throughput data available.");
            return;
        }

        string json = JsonConvert.SerializeObject(pdcpData, Formatting.Indented);

        Debug.Log("PDCP Throughput JSON: " + json);

        string folderPath = Application.streamingAssetsPath;
        string filePath = folderPath + "/pdcpThroughput.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllText(filePath, json);
        Debug.Log("JSON saved to: " + filePath);
    }
    public void LatencyJson()
    {
        List<latencyTotal> latencyData = messageManager.GetLatencyTotalInfo();

        if (latencyData == null || latencyData.Count == 0)
        {
            Debug.LogWarning("No pkt data available.");
            return;
        }

        string json = JsonConvert.SerializeObject(latencyData, Formatting.Indented);

        Debug.Log("pkt JSON: " + json);

        string folderPath = Application.streamingAssetsPath;
        string filePath = folderPath + "/latencyTotal.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllText(filePath, json);
        Debug.Log("JSON saved to: " + filePath);
    }

    public void PktJson()
    {
        List<pkt> pktData = messageManager.getPktInfo();

        string json = JsonConvert.SerializeObject(pktData, Formatting.Indented);

        Debug.Log("pkt JSON: " + json);

        string folderPath = Application.streamingAssetsPath;
        string filePath = folderPath + "/pktInfo.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllText(filePath, json);
        Debug.Log("JSON saved to: " + filePath);
    }
}