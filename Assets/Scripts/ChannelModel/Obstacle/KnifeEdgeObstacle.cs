using System;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class KnifeEdgeObstacle : MonoBehaviour
{

    public double ComputeSingleKnifeEdge(float frequency, Vector3 RayA, Vector3 RayB)
    {
        double skeLoss = 0;
        double waveLength = 300000000 / frequency;
        double angle = Vector3.Angle(RayA, RayB) * Mathf.Deg2Rad;
        double v = angle * Math.Pow((2/(waveLength*(1/ RayA.magnitude+1/ RayB.magnitude))),0.5);

        double cosV = FresnelCos(v);
        double sinV = FresnelSin(v);
        double a = 1.0 - cosV - sinV;
        double b = cosV - sinV;

        skeLoss = -20 * Math.Log10(Math.Pow(Math.Pow(a, 2) + Math.Pow(b, 2), 0.5)/2);
        //return v;
        return skeLoss;
    }

    public double ComputeDoubleKnifeEdgeLc(double lossA, double lossB, Vector3 RayA, Vector3 RayB, Vector3 RayC, Vector3 start, Vector3 dest)
    {
        double totalLoss = 0;
        Vector3 point1 = start + RayA;
        Vector3 point2 = point1 + RayB;
        Vector3 startToDest = dest - start;

        Vector3 proj1 = start + Vector3.Project(point1 - start, startToDest);
        Vector3 proj2 = start + Vector3.Project(point2 - start, startToDest);

        double a = Vector3.Distance(start, proj1);
        double b = Vector3.Distance(proj1, proj2);
        double c = Vector3.Distance(proj2, dest);

        double Lc = 10 * Math.Log10( ((a + b) * (b + c)) / (b * (a + b + c)) );

        totalLoss = lossA + lossB + Lc;

        return totalLoss;
    }



    public double ComputeDoubleKnifeEdgeTc(double lossA, double lossB, Vector3 RayA, Vector3 RayB, Vector3 RayC, Vector3 start, Vector3 dest, float frequency)
    {
        double totalLoss = 0;
        Vector3 point1 = start + RayA;
        Vector3 point2 = point1 + RayB;
        Vector3 startToDest = dest - start;

        Vector3 proj1 = start + Vector3.Project(point1 - start, startToDest);
        Vector3 proj2 = start + Vector3.Project(point2 - start, startToDest);

        double a = Vector3.Distance(start, proj1);
        double b = Vector3.Distance(proj1, proj2);
        double c = Vector3.Distance(proj2, dest);

        double h1 = Vector3.Distance(point1, proj1);
        double h2 = Vector3.Distance(point2, proj2);

        double alpha = Math.Atan(Math.Pow((b * (a + b + c)/ (a * c)), 0.5));

        double wavelength = 300000000 / frequency;

        double p = Math.Pow(( (2 * (a + b + c)) / (wavelength * (b + c) * a)), 0.5) * h1;
        double q = Math.Pow(( (2 * (a + b + c)) / (wavelength * (a + b) * c)), 0.5) * h2;

        double Tc = (12 - 20 * Math.Log10( 2 / (1 - alpha / Math.PI))) * Math.Pow((q / p), 2 * p);

        totalLoss = lossA + lossB - Tc;

        return totalLoss;
    }

    public double FresnelCos(double x)
    {
        int n = 100;
        double step = x / n;
        double sumC = 0f;

        for (int i = 0; i < n; i++)
        {
            double t = i * step;
            sumC += Math.Cos(Math.PI * t * t / 2) * step;
        }

        return sumC;
    }

    public double FresnelSin(double x)
    {
        int n = 100;
        double step = x / n;
        double sumS = 0f;

        for (int i = 0; i < n; i++)
        {
            double t = i * step;
            sumS += Math.Sin(Math.PI * t * t / 2) * step;
        }

        return sumS;
    }
}
