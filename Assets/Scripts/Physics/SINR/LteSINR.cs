using System;
using System.Collections.Generic;
using UnityEngine;
using static TransmissionParameterManager;

public class LteSINR : MonoBehaviour
{
    
    public bool fadingEnabled = true;
    public double recvPower = 0.0;
    public double fading;

    //public UeBase Ue;
    public JakesFading jakesFading;
    public DopplerSpeed dopplerSpeed;
    public TotalRecvPower totalRecvPower;
    public List<double> GetSINR(bool isUpload, TransmissionParameter transmissionParameter)
    {
        //Debug.Log(transmissionParameter.numBands);
        List<double> snrVector = new List<double>(new double[transmissionParameter.numBands]);

        double noiseFigure = transmissionParameter.noiseFigure;
        double thermalNoise = transmissionParameter.thermalNoise;

        Vector3 ueCoord = transmissionParameter.positionA;
        Vector3 enbCoord = transmissionParameter.positionB;

        double txPowerUl = transmissionParameter.txPowerA;
        double txPowerDl = transmissionParameter.txPowerB;

        double frequency = transmissionParameter.frequency;

        //if (isUpload ) {recvPower = txPowerUl; }
        //else {recvPower = txPowerDl; }

        recvPower = totalRecvPower.computeTotalRecvpower(isUpload, transmissionParameter);

        float speed = dopplerSpeed.computeDopplerSpeed(transmissionParameter);

        for (int i = 0; i < transmissionParameter.numBands; i++)
        {
            if (fadingEnabled)
            {
                fading = jakesFading.JakesFadingComputation(transmissionParameter, i, speed, true);
            }

            double interference = 0.0; 
            double noise = thermalNoise + noiseFigure;
            double sinr = recvPower + fading - interference - noise;
            snrVector[i] = sinr;
        }

        return snrVector;
    }
}

