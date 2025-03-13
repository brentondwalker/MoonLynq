using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static TransmissionParameterManager;

public class CQI_Base : MonoBehaviour
{
    public CQI_Computaion cqiComputation;
    public LteSINR lteSINR;
    public List<double> snrv = new List<double>();
    public int cqi;
    public double meanSNR;
    public int txmode = 2;
    //public UeBase ueBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public int GetCqi(TransmissionParameter parameter)
    {
        snrv.Clear();
        snrv = lteSINR.GetSINR(true, parameter);
        meanSNR = cqiComputation.MeanSnr(snrv);
        cqi = cqiComputation.GetCqi(txmode, meanSNR);
        //cqi = cqiComputation.GetCqi(txmode, 32);
        return cqi;
    }
}
