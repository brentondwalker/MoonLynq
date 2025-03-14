using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CQI_Computaion : MonoBehaviour
{
    private List<double> baseMin;
    public double targetBler = 0.01; 
    public PisaData pisaData;

    public int minSnr;
    public int maxSnr;
    public int nMcs;

    private void Start()
    {
        baseMin = new List<double>(new double[pisaData.nMcs()]);
        minSnr = pisaData.MinSnr();
        maxSnr = pisaData.MaxSnr();
        nMcs = pisaData.nMcs();
    }


    public int GetCqi(int txMode, double snr)
    {
        int newsnr = (int)Math.Floor(snr + 0.5);
        if (newsnr < minSnr)
            return 0;
        if (newsnr > maxSnr)
            return 15;

        baseMin = new List<double>(new double[pisaData.nMcs()]);
        List<double> min = new List<double>(baseMin);

        int found = 0;
        double low = 2.0;

        for (int i = 0; i < nMcs; i++)
        {
            double tmp = pisaData.GetBler(txMode, i+1, newsnr);
            double diff = targetBler - tmp;
            min[i] = (diff > 0) ? diff : (diff * -1);
            if (low >= min[i])
            {
                found = i;
                low = min[i];
            }
        }
        int cqiResult = found + 1;
        //UnityEngine.Debug.Log("cqi" + cqiResult);
        return cqiResult;
    }

    public double MeanSnr(List<double> snr)
    {
        if (snr == null || snr.Count == 0)
            return 0;

        double sum = 0;
        foreach (double value in snr)
        {
            sum += value;
        }

        return sum / snr.Count;
    }

}