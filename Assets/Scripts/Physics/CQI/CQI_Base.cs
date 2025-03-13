using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CQI_Base : MonoBehaviour
{
    public CQI_Computaion cqiComputation;
    public LteSINR lteSINR;
    public List<double> snrv = new List<double>();
    public int cqi;
    public double meanSNR;
    public int txmode = 2;
    public UeBase ueBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 0.1f;
    // Update is called once per frame
    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }
    void CustomUpdate()
    {
        snrv.Clear();
        for (int i = 0; i < ueBase.transmissionParameters.Length; i++) {
            snrv = lteSINR.GetSINR(true, ueBase.transmissionParameters[i]);
        }
        meanSNR = cqiComputation.MeanSnr(snrv);
        cqi = cqiComputation.GetCqi(txmode, meanSNR);
        //cqi = cqiComputation.GetCqi(txmode, 32);
    }
}
