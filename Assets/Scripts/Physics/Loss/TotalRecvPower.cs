using UnityEngine;
using System;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class TotalRecvPower : MonoBehaviour
{
    public LOS_Ray los;
    public ReflectionRay reflection;
    public DiffractionRay diffraction;
    public UeInfo ueInfo;

    public double antennaGainTx = 0.0;
    public double antennaGainRx = 18.0;
    public double cableLoss = 2.0;

    public bool enableReflection = true;
    public bool enableDiffraction = true;

    public double computeTotalRecvpower(bool isUpload)
    {
        double distance = Vector3.Distance(ueInfo.ueObject.transform.position, ueInfo.TargetEnb.enbObject.transform.position);

        double losPower = 0;
        double diffractionPower = 0;
        double[] reflectionPower = new double[reflection.totalLoss.Length];

        double txPower = 0;
        txPower += antennaGainTx + antennaGainRx - cableLoss;

        if (isUpload)
        {   
            txPower += ueInfo.txPowerBaseUl;
            losPower = txPower + los.totalLossForwardInDB - LosPathLoss(distance);
        }
        else 
        {           
            txPower += ueInfo.TargetEnb.txPower;
            losPower = txPower + los.totalLossReverseInDB - LosPathLoss(distance);
        }

        diffractionPower = txPower - diffraction.totalLoss;
        if (double.IsNaN(diffractionPower)) diffractionPower = double.NegativeInfinity;

        for (int i = 0; i < reflection.totalLoss.Length; i++) 
        {
            reflectionPower[i] = txPower - reflection.totalLoss[i];
            if (double.IsNaN(reflectionPower[i])) reflectionPower[i] = double.NegativeInfinity;
        }

        //Debug.Log("reflectionPower: " + reflectionPower);
        //Debug.Log("diffractionPower: " + diffractionPower);
        //Debug.Log("losPower:" + losPower);


        return ComputeCoherentPower(losPower, diffractionPower, reflectionPower, distance);
    }

    double ComputeCoherentPower(double losPower, double diffractionPower, double[] reflectionPower, double distance)
    {
        double E_real = 0, E_imag = 0;

        double E_los = Math.Sqrt(PowerCalculator.dBToLinear(losPower));
        double phase_los = GenerateRandomPhase(distance);
        E_real += E_los * Math.Cos(phase_los);
        E_imag += E_los * Math.Sin(phase_los);

        if (enableDiffraction)
        {
            double E_diffraction = Math.Sqrt(PowerCalculator.dBToLinear(diffractionPower));
            double phase_diffraction = GenerateRandomPhase(Math.Pow(distance, 2));
            E_real += E_diffraction * Math.Cos(phase_diffraction);
            E_imag += E_diffraction * Math.Sin(phase_diffraction);
        }

        if (enableReflection)
        {
            for (int i = 0; i < reflectionPower.Length; i++)
            {
                double E_ref = Math.Sqrt(PowerCalculator.dBToLinear(reflectionPower[i]));
                double phase_ref = GenerateRandomPhase(Math.Pow(distance, 0.1 * i));
                E_real += E_ref * Math.Cos(phase_ref);
                E_imag += E_ref * Math.Sin(phase_ref);
            }
        }

        double E_total = Math.Sqrt(E_real * E_real + E_imag * E_imag);

        return PowerCalculator.linearToDb(E_total * E_total);
    }

    public double GenerateRandomPhase(double distance)
    {
        System.Random rand = new System.Random(distance.GetHashCode());
        return rand.NextDouble() * 2 * Math.PI;
    }

    double LosPathLoss(double distance) { return 20 * Math.Log10(distance) + 20 * Math.Log10(ueInfo.frequency) - 147.55;; }

}
