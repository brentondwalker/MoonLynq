using UnityEngine;
using System;
using Unity.VisualScripting;
using static TransmissionParameterManager;

public class TotalRecvPower : MonoBehaviour
{
    public LOS_Ray los;
    public ReflectionRay reflection;
    public DiffractionRay diffraction;
    //public UeBase ueBase;

    public double antennaGainTx = 0.0;
    public double antennaGainRx = 18.0;

    public bool enableReflection = true;
    public bool enableDiffraction = true;

    public double computeTotalRecvpower(bool isUpload, TransmissionParameter transmissionParameter)
    {
        Vector3 start = transmissionParameter.positionA;
        Vector3 dest = transmissionParameter.positionB;
        float frequency = transmissionParameter.frequency;

        double distance = Vector3.Distance(start, dest);

        double losPower = 0;
        double diffractionPower = 0;
        double[] reflectionPower = new double[reflection.lineCount];

        double txPower = 0;
        txPower += antennaGainTx + antennaGainRx - transmissionParameter.cableLoss;

        if (isUpload)
        {   
            txPower += transmissionParameter.txPowerA;
            losPower = txPower + los.totalLossForwardInDB - LosPathLoss(distance, frequency);
        }
        else 
        {           
            txPower += transmissionParameter.txPowerB;
            losPower = txPower + los.totalLossReverseInDB - LosPathLoss(distance, frequency);
        }

        diffractionPower = txPower - diffraction.ComputeNlosDiffraction(start, dest, frequency);
        if (double.IsNaN(diffractionPower)) diffractionPower = double.NegativeInfinity;

        double[] absorptionLoss = new double[reflection.lineCount];
        absorptionLoss = reflection.ComputeNlosReflection(start, dest, frequency);
        for (int i = 0; i < reflection.lineCount; i++) 
        {
            reflectionPower[i] = txPower - absorptionLoss[i];
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

    double LosPathLoss(double distance, float frequency) { return 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55; }

}
