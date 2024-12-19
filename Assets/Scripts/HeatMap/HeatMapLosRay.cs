using System.Collections.Generic;
using UnityEngine;

public class HeatMapLosRay : MonoBehaviour
{
    public GameObject targetGnb;
    Vector3 gnbPosition;

    public float frequency = 2000000000;
    private List<Vector3> dataPoints;
    private List<float> totalLosses;

    public int gridSpacing = 10;
    private float rangeX;
    private float rangeZ;
    private float startX;
    private float startZ;
    private float startY;

    public ObstacleLos obstacleLos;
    public string collisionInfo;

    public HeatMap heatMap;
    public PlaneInfo planeInfo;
    public int textureSize = 256;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rangeX = planeInfo.width;
        rangeZ = planeInfo.height;
        startX = planeInfo.vertice.x;
        startZ = planeInfo.vertice.z;
        startY = planeInfo.vertice.y;


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


    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }
    void CustomUpdate()
    {
        gnbPosition = targetGnb.transform.position;
        GenerateDataPoints();
        totalLosses.Clear();
        foreach (var point in dataPoints)
        {
            collisionInfo = obstacleLos.CheckLineCollision(frequency, point, gnbPosition);
            double totalLoss = obstacleLos.getTotalLossReverseInDB();
            totalLosses.Add((float)totalLoss);
            //Debug.Log("Heat Map Loss added:" + totalLoss + " for point:" + point.x + " " + point.z);
            //Debug.Log(collisionInfo);
            //Debug.DrawLine(point, gnbPosition, Color.red, 100f);
        }
        heatMap.GenerateHeatmap(dataPoints,totalLosses,textureSize);
    }

    void GenerateDataPoints()
    {
        dataPoints.Clear();
        for (float x = startX-rangeX; x <= startX; x += gridSpacing)
        {
            for (float z = startZ-rangeZ; z <= startZ; z += gridSpacing)
            {
                Vector3 point = new Vector3(x, startY, z);
                dataPoints.Add(point);
            }
        }
    }
}
