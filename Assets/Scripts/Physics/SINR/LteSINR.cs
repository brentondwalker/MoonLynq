using System;
using System.Collections.Generic;
using UnityEngine;

public class LteSINR : MonoBehaviour
{
    private double noiseFigure = 5.0;
    private double thermalNoise = -104;
    public bool fadingEnabled = true;
    public double recvPower = 0.0;
    public double fading;

    public UeInfo Ue;
    public JakesFading jakesFading;
    public DopplerSpeed dopplerSpeed;
    public TotalRecvPower totalRecvPower;
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

        recvPower = totalRecvPower.computeTotalRecvpower(isUpload);

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
}

