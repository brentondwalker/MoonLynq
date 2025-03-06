using System;
using System.Collections.Generic;
using UnityEngine;

public class LteSINR : MonoBehaviour
{
    public double antennaGainTx = 0.0;
    public double antennaGainRx = 0.0;
    public double recvPower = 0.0;
    private double noiseFigure = 5.0;
    private double cableLoss = 2.0;
    private double thermalNoise = -104;
    public bool fadingEnabled = true;

    public double fading;

    public UeInfo Ue;
    public JakesFading jakesFading;
    public DopplerSpeed dopplerSpeed;
    public List<double> GetSINR(bool isUpload)
    {
        List<double> snrVector = new List<double>(new double[Ue.numBands]);

        Vector3 ueCoord = Ue.prefabPosition;
        Vector3 enbCoord = Ue.TargetEnb.enbMobiltiyLocal;

        double txPowerUe = Ue.txPowerBaseUl;
        double txPowerENB = Ue.TargetEnb.txPower;

        double frequency = Ue.frequency;

        if (isUpload ) {recvPower = txPowerUe; }
        else {recvPower = txPowerENB; }

        double attenuation = GetAttenuation(ueCoord, enbCoord, frequency, isUpload); 

        recvPower -= attenuation; 
        recvPower += antennaGainTx;
        recvPower += antennaGainRx;
        recvPower -= cableLoss;

        float speed = dopplerSpeed.computeDopplerSpeed(Ue.ueObject, Ue.TargetEnb.enbObject);

        for (int i = 0; i < Ue.numBands; i++)
        {
            if (fadingEnabled)
            {
                fading = jakesFading.JakesFadingComputation(Ue.numBands, i, speed, true, Ue);
            }

            double interference = 0.0; 
            double noise = thermalNoise + noiseFigure;
            double sinr = recvPower + fading - interference - noise;
            snrVector[i] = sinr;
        }

        return snrVector;
    }

    private double GetAttenuation(Vector3 ueCoord, Vector3 enbCoord, double frequency, bool isUpload)
    {
        double distance = Vector3.Distance(ueCoord, enbCoord);
        double pathLoss = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
        double obstacleLoss = 0;
        LOS_Ray visualLine = Ue.LOS_Ray;
        if (isUpload) { obstacleLoss = visualLine.totalLossForwardInDB; }
        else { obstacleLoss = visualLine.totalLossReverseInDB; }
        double attenuation = -obstacleLoss + pathLoss;
        //Debug.Log("distance: " + distance);
        //Debug.Log("pathLoss: " + pathLoss);
        //Debug.Log("attenuation: " + attenuation);
        return attenuation;
    }
}

