using System;
using System.Collections.Generic;
using UnityEngine;

public class LteSINR : MonoBehaviour
{
    public double antennaGainTx = 0.0;
    public double antennaGainRx = 0.0;
    private double noiseFigure = 5.0;
    private double cableLoss = 2.0;
    private double thermalNoise = -104;
    private int numBands = 1; 
    private bool fadingEnabled = true;

    public UeInfo Ue;

    public List<double> GetSINR(double speed, bool isUpload)
    {
        List<double> snrVector = new List<double>(new double[numBands]);

        Vector3 ueCoord = Ue.prefabPosition;
        Vector3 enbCoord = Ue.TargetGnb.gnbMobiltiyLocal;

        double txPowerUe = Ue.txPowerBaseUl;
        double txPowerENB = Ue.TargetGnb.txPower;

        double frequency = Ue.frequency;

        double recvPower = 0;

        if (isUpload ) {recvPower = txPowerUe; }
        else {recvPower = txPowerENB; }

        double attenuation = GetAttenuation(ueCoord, enbCoord, frequency, isUpload); 

        recvPower -= attenuation; 
        recvPower += antennaGainTx;
        recvPower += antennaGainRx;
        recvPower -= cableLoss;

        double fadingAttenuation = 0.0;
        if (fadingEnabled)
        {
            fadingAttenuation = ApplyFading(ueCoord, enbCoord, speed);
        }
        recvPower += fadingAttenuation;

        for (int i = 0; i < numBands; i++)
        {
            double interference = 0.0; 
            double noise = thermalNoise + noiseFigure;
            double sinr = (recvPower - interference) - noise;
            snrVector[i] = sinr;
        }

        return snrVector;
    }

    private double GetAttenuation(Vector3 ueCoord, Vector3 enbCoord, double frequency, bool isUpload)
    {
        double distance = Vector3.Distance(ueCoord, enbCoord);
        double pathLoss = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
        double obstacleLoss = 0;
        VisualLine visualLine = Ue.VisualLine;
        if (isUpload) { obstacleLoss = visualLine.totalLossForwardInDB; }
        else { obstacleLoss = visualLine.totalLossReverseInDB; }
        double attenuation = -obstacleLoss + pathLoss;
        //Debug.Log("distance: " + distance);
        //Debug.Log("pathLoss: " + pathLoss);
        //Debug.Log("attenuation: " + attenuation);
        return attenuation;
    }

    private double ApplyFading(Vector3 ueCoord, Vector3 enbCoord, double speed)
    {
        // 应用 Rayleigh 或 Jakes 衰落模型
        double fading = 5.0;
        return fading;
    }


}

