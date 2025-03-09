using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;

public class HeatMapRayBase : MonoBehaviour
{
    public GameObject targetGnb;
    Vector3 gnbPosition;

    public float frequency = 2000000000;
    public List<Vector3> dataPoints;
    public List<float> losTotalLosses;
    public List<float> reflectionTotalLosses;
    public List<float> diffractionTotalLosses;
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

    public ReflectionRay reflectionRay;
    public DiffractionRay diffractionRay;

    public HeatMap heatMap;
    public PlaneInfo planeInfo;
    public int textureSize = 256;

    public DropdownManager dropdownManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


        dataPoints = new List<Vector3>();
        losTotalLosses = new List<float>();
        diffractionTotalLosses = new List<float>();
        reflectionTotalLosses = new List<float> ();


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

    public void UpdateHeatmap()
    {
        string mode = dropdownManager.currentMode;

        if (mode == "LoS") UpdateLosHeatMap();
        if (mode == "Reflection") UpdateReflectionHeatMap();
        if (mode == "Diffraction") UpdateDiffractionHeatMap();
        if (mode == "Total") UpdateTotalHeatMap();

        RegenerateHeatMap();
    }

    public void UpdateLosHeatMap()
    {
        rangeX = planeInfo.width;
        rangeZ = planeInfo.height;
        startX = planeInfo.vertice.x;
        startZ = planeInfo.vertice.z;
        startY = planeInfo.vertice.y;
        gnbPosition = targetGnb.transform.position;
        GenerateDataPoints();
        losTotalLosses.Clear();
        foreach (var point in dataPoints)
        {
            collisionInfo = obstacleLos.CheckLineCollision(frequency, point, gnbPosition);
            //double totalLoss = obstacleLos.getTotalLossReverseInDB();
            double totalLoss = obstacleLos.getTotalLossForwardInDB();
            double distance = Vector3.Distance(point, gnbPosition);
            double pathLoss = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
            totalLoss -= pathLoss;
            losTotalLosses.Add((float)totalLoss);
            //Debug.Log("Heat Map Loss added:" + totalLoss + " for point:" + point.x + " " + point.z);
            //Debug.Log(collisionInfo);
            //Debug.DrawLine(point, gnbPosition, Color.red, 100f);
        }
        heatMap.GenerateHeatMap(dataPoints, losTotalLosses, textureSize);
    }

    public void UpdateReflectionHeatMap()
    {
        rangeX = planeInfo.width;
        rangeZ = planeInfo.height;
        startX = planeInfo.vertice.x;
        startZ = planeInfo.vertice.z;
        startY = planeInfo.vertice.y;
        gnbPosition = targetGnb.transform.position;
        GenerateDataPoints();
        reflectionTotalLosses.Clear();
        foreach (var point in dataPoints)
        {
            double[] totalLoss = reflectionRay.ComputeNlosReflection(point, gnbPosition, frequency);
            for (int i = 0; i < totalLoss.Length; i++)
            {
                if (double.IsNaN(totalLoss[i])) totalLoss[i] = double.NegativeInfinity;
                else totalLoss[i] = -totalLoss[i];
            }
            double totalLossCoherent = ComputeCoherentPower(totalLoss,Vector3.Distance(point, gnbPosition));
            reflectionTotalLosses.Add((float)totalLossCoherent);
        }
        heatMap.GenerateHeatMap(dataPoints, reflectionTotalLosses, textureSize);
    }

    public void UpdateDiffractionHeatMap()
    {
        rangeX = planeInfo.width;
        rangeZ = planeInfo.height;
        startX = planeInfo.vertice.x;
        startZ = planeInfo.vertice.z;
        startY = planeInfo.vertice.y;
        gnbPosition = targetGnb.transform.position;
        GenerateDataPoints();
        diffractionTotalLosses.Clear();
        foreach (var point in dataPoints)
        {
            double totalLoss = diffractionRay.ComputeNlosDiffraction(point, gnbPosition, frequency);
            if (double.IsNaN(totalLoss)) totalLoss = double.NegativeInfinity;
            else totalLoss = -totalLoss;
            diffractionTotalLosses.Add((float)totalLoss);
        }
        heatMap.GenerateHeatMap(dataPoints, diffractionTotalLosses, textureSize);
    }

    public void UpdateTotalHeatMap()
    {
        UpdateLosHeatMap();
        UpdateReflectionHeatMap();
        UpdateDiffractionHeatMap();

        totalLosses.Clear();

        //Debug.Log("LosHeatMap Length: " + losTotalLosses.Count);
        //Debug.Log("ReflectionHeatMap Length: " + reflectionTotalLosses.Count);
        //Debug.Log("DiffractionHeatMap Length: " + diffractionTotalLosses.Count);

        for (int i = 0; i < losTotalLosses.Count; i++)
        {
            //float combinedLoss = losTotalLosses[i] + reflectionTotalLosses[i] + diffractionTotalLosses[i];
            double distance = Vector3.Distance(dataPoints[i], gnbPosition);
            float combinedLoss = (float)ComputeCoherentPower(losTotalLosses[i], reflectionTotalLosses[i], diffractionTotalLosses[i], distance);
            totalLosses.Add(combinedLoss);
        }
        heatMap.GenerateHeatMap(dataPoints, totalLosses, textureSize);
    }

    public void RegenerateHeatMap()
    {
        string mode = dropdownManager.currentMode;
        if (mode == "LoS") heatMap.GenerateHeatMap(dataPoints, losTotalLosses, textureSize);
        if (mode == "Reflection") heatMap.GenerateHeatMap(dataPoints, reflectionTotalLosses, textureSize);
        if (mode == "Diffraction") heatMap.GenerateHeatMap(dataPoints, diffractionTotalLosses, textureSize);
        if (mode == "Total") heatMap.GenerateHeatMap(dataPoints, totalLosses, textureSize);
    }

    void GenerateDataPoints()
    {
        dataPoints.Clear();

        rows = Mathf.RoundToInt((float)planeInfo.height / gridSpacing);
        cols = Mathf.RoundToInt((float)planeInfo.width / gridSpacing);

        //Debug.Log("rows: " + rows);
        //Debug.Log("cols: " + cols);

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

    double ComputeCoherentPower(double losPower, double diffractionPower, double reflectionPower, double distance)
    {
        double E_real = 0, E_imag = 0;

        double E_los = Math.Sqrt(PowerCalculator.dBToLinear(losPower));
        double phase_los = GenerateRandomPhase(distance);
        E_real += E_los * Math.Cos(phase_los);
        E_imag += E_los * Math.Sin(phase_los);


            double E_diffraction = Math.Sqrt(PowerCalculator.dBToLinear(diffractionPower));
            double phase_diffraction = GenerateRandomPhase(Math.Pow(distance, 2));
            E_real += E_diffraction * Math.Cos(phase_diffraction);
            E_imag += E_diffraction * Math.Sin(phase_diffraction);


                double E_ref = Math.Sqrt(PowerCalculator.dBToLinear(reflectionPower));
                double phase_ref = GenerateRandomPhase(Math.Pow(distance, 0.1));
                E_real += E_ref * Math.Cos(phase_ref);
                E_imag += E_ref * Math.Sin(phase_ref);
           
        

        double E_total = Math.Sqrt(E_real * E_real + E_imag * E_imag);

        return PowerCalculator.linearToDb(E_total * E_total);
    }


    double ComputeCoherentPower(double[] reflectionPower, double distance)
    {
        double E_real = 0, E_imag = 0;

        for (int i = 0; i < reflectionPower.Length; i++)
        {
            double E_ref = Math.Sqrt(PowerCalculator.dBToLinear(reflectionPower[i]));
            double phase_ref = GenerateRandomPhase(Math.Pow(distance, 0.1 * i));
            E_real += E_ref * Math.Cos(phase_ref);
            E_imag += E_ref * Math.Sin(phase_ref);
        }


        double E_total = Math.Sqrt(E_real * E_real + E_imag * E_imag);

        return PowerCalculator.linearToDb(E_total * E_total);
    }

    public double GenerateRandomPhase(double distance)
    {
        System.Random rand = new System.Random(distance.GetHashCode());
        return rand.NextDouble() * 2 * Math.PI;
    }
}
