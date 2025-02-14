using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using static MGData;

public class MGDataToJSON : MonoBehaviour
{
    public MGMessageManager messageManager;

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
}