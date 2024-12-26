using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class HeatmapJSON : MonoBehaviour
{
    [SerializeField] public string filename = "heatmap_data.json";

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 1f;

    public HeatMapLosRay heatMapLosRay; 

    public string jsonData;
    public bool writeJsonToLocal = true;
    public float distanceConvert = 1;

    void Start()
    {
        distanceConvert = 1 / ScenarioScale.staticScale;
    }
    public void WriteHeatmapData()
    {
        List<Vector3> dataPoints = heatMapLosRay.dataPoints;
        List<float> totalLosses = heatMapLosRay.totalLosses;

        if (dataPoints == null || totalLosses == null || dataPoints.Count != totalLosses.Count)
        {
            Debug.LogError("Data points or losses are invalid.");
            return;
        }

        HeatmapData heatmap = new HeatmapData
        {
            rows = Mathf.RoundToInt((float)heatMapLosRay.planeInfo.height / heatMapLosRay.gridSpacing),
            cols = Mathf.RoundToInt((float)heatMapLosRay.planeInfo.width / heatMapLosRay.gridSpacing),
            data = ConvertTo2DArray(dataPoints, totalLosses, heatMapLosRay.gridSpacing)
        };

        string json = JsonConvert.SerializeObject(heatmap, Formatting.Indented);

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        if (writeJsonToLocal)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(json);
            }

            Debug.Log("Heatmap JSON data written to: " + filePath);
        }

    }

    private float[,] ConvertTo2DArray(List<Vector3> points, List<float> values, float spacing)
    {
        int rows = Mathf.RoundToInt((float)heatMapLosRay.planeInfo.height / spacing);
        int cols = Mathf.RoundToInt((float)heatMapLosRay.planeInfo.width / spacing);
        float[,] data = new float[rows, cols];

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float value = values[i];

            int row = Mathf.RoundToInt((point.z - heatMapLosRay.startZ + heatMapLosRay.rangeZ) / spacing);
            int col = Mathf.RoundToInt((point.x - heatMapLosRay.startX + heatMapLosRay.rangeX) / spacing);

            if (row >= 0 && row <= rows && col >= 0 && col <= cols)
            {
                data[row, col] = value;
            }
        }

        return data;
    }
}

[System.Serializable]
public class HeatmapData
{
    public int rows; 
    public int cols; 
    public float[,] data; 
}
