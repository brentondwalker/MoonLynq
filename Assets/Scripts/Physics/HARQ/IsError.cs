using System.Collections.Generic;
using System;
using UnityEngine;

public class IsError : MonoBehaviour
{
    //private double harqReduction = 0.8; 
    public double bler;

    public PisaData pisaData;
    //public GetBLER_TU getBler;

    public double GetErrorRate(int cqi, int numBands, List<double> snrV)
    {
        if (cqi < 0 || cqi > 15) throw new Exception("Invalid CQI value");

        double finalSuccess = 1.0;

        if (cqi == 0) 
        { return 1; }

        for (int i = 0; i < snrV.Count; i++)
        {
            double snr = snrV[i];
            //int newsnr = (int)Math.Floor(snr + 0.5);
            //bler = (snr < pisaData.MinSnr()) ? 1.0 : pisaData.GetBler(0, cqi, newsnr);
            bler = (snr < pisaData.MinSnr()) ? 1.0 : pisaData.GetBler(snr, cqi);
            finalSuccess *= Math.Pow(1 - bler, 1);
        }

        double per = 1 - finalSuccess;

        return per;
    }

    public double GetPktErrorRate (int pktLength, double per, float throughput)
    {
        double pktf = throughput / pktLength / 8 / 100;
        double pktPer = Math.Pow(per, 1 / pktf);
        return pktPer;
    }
}
