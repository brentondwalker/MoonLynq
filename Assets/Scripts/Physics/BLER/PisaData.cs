using System;
using UnityEngine;

public class PisaData : MonoBehaviour
{
    private double[,,] blerCurves_;
    private double[] channel_ = new double[10000]; 
    private int blerShift_ = 5; 
    private System.Random rng = new System.Random();
    public GetBLER_TU getBler;

    public PisaData()
    {
        //Array.Copy(BlerCurvesData.BlerCurvesNew, blerCurves_, BlerCurvesData.BlerCurvesNew.Length);

        blerCurves_ = BlerCurvesData.BlerCurvesNew;

        for (int i = 0; i < 1000; i++)
        {
            double x = NormalRandom(0, 0.5);
            double y = NormalRandom(0, 0.5);
            channel_[i] = (x * x) + (y * y);
        }

    }


    public double GetChannel(uint i)
    {
        i = i % (uint)channel_.Length;
        return channel_[i];
    }


    private double NormalRandom(double mean, double stdDev)
    {
        double u1 = 1.0 - rng.NextDouble(); 
        double u2 = 1.0 - rng.NextDouble(); 
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); 
        return mean + stdDev * randStdNormal;
    }

    public double GetBler(int i, int j, int k)
    {
        return getBler.GetBLER(k + blerShift_, j);
    }

    public double GetBler(double snr, int cqi)
    {
        return getBler.GetBLER(snr + blerShift_, cqi);
    }

    public int MinSnr()
    {
        return -14 - blerShift_; 
    }

    public int MaxSnr()
    {
        return 40 - blerShift_;
    }

    public int nMcs() { return 15; }
}