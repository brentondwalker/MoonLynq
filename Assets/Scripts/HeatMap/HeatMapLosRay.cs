using System.Collections.Generic;
using UnityEngine;

public class HeatMapLosRay : MonoBehaviour
{
    public GameObject targetGnb;
    Vector3 gnbPosition;

    public float frequency = 2000000000;
    public List<Vector3> dataPoints;
    public List<float> totalLosses;

    public float gridSpacing = 10;
    public float rangeX;
    public float rangeZ;
    public float startX;
    public float startZ;
    public float startY;

    public int rows = 1;
    public int cols = 1;

    public ObstacleLos obstacleLos;
    public string collisionInfo;

    public HeatMap heatMap;
    public PlaneInfo planeInfo;
    public int textureSize = 256;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


        dataPoints = new List<Vector3>();
        totalLosses = new List<float>();


        //GenerateDataPoints();
        //foreach (var point in dataPoints)
        //{
            //collisionInfo = obstacleLos.CheckLineCollision(frequency, point, gnbPosition);
            //double totalLoss = obstacleLos.getTotalLossForwardInDB();
            //totalLosses.Add((float)totalLoss);
            //Debug.Log("Heat Map Loss added:" + totalLoss + " for point:" + point.x + " " + point.z);
            //Debug.Log(collisionInfo);
            //Debug.DrawLine(point, gnbPosition, Color.red, 100f);
        //}
        //heatMap.GenerateHeatmap(dataPoints,totalLosses,textureSize);
    }


    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 2;


    //void Update()
    //{
        //timeSinceLastUpdate = Time.time - lastUpdateTime;
        //if (timeSinceLastUpdate >= updateInterval)
        //{
        //    CustomUpdate();
        //    lastUpdateTime = Time.time;
        //}
    //}
    public void UpdateHeatMap()
    {
        rangeX = planeInfo.width;
        rangeZ = planeInfo.height;
        startX = planeInfo.vertice.x;
        startZ = planeInfo.vertice.z;
        startY = planeInfo.vertice.y;
        gnbPosition = targetGnb.transform.position;
        GenerateDataPoints();
        totalLosses.Clear();
        foreach (var point in dataPoints)
        {
            collisionInfo = obstacleLos.CheckLineCollision(frequency, point, gnbPosition);
            //double totalLoss = obstacleLos.getTotalLossReverseInDB();
            double totalLoss = obstacleLos.getTotalLossForwardInDB();
            totalLosses.Add((float)totalLoss);
            //Debug.Log("Heat Map Loss added:" + totalLoss + " for point:" + point.x + " " + point.z);
            //Debug.Log(collisionInfo);
            //Debug.DrawLine(point, gnbPosition, Color.red, 100f);
        }
        heatMap.GenerateHeatMap(dataPoints,totalLosses,textureSize);
    }

    public void RegenerateHeatMap()
    {
        heatMap.GenerateHeatMap(dataPoints, totalLosses, textureSize);
    }

    void GenerateDataPoints()
    {
        dataPoints.Clear();

        rows = Mathf.RoundToInt((float)planeInfo.height / gridSpacing);
        cols = Mathf.RoundToInt((float)planeInfo.width / gridSpacing);

        Debug.Log("rows: " + rows);
        Debug.Log("cols: " + cols);

        for (int i = 0; i <= rows; i++)
        {
            for (int j = 0; j <= cols; j++)
            {
                float x = startX - rangeX + j * gridSpacing;
                float z = startZ - i * gridSpacing;

                Vector3 point = new Vector3(x, startY, z);
                dataPoints.Add(point);
            }
        }
    }
}
