using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CQI_Test : MonoBehaviour
{
    public CQI_Computaion cqiComputation;
    public LteSINR lteSINR;
    public List<double> snrv = new List<double>();
    public int cqi;
    public double meanSNR;
    public int txmode = 2;

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
        snrv = lteSINR.GetSINR(true);
        meanSNR = cqiComputation.MeanSnr(snrv);
        cqi = cqiComputation.GetCqi(txmode, meanSNR);
        //cqi = cqiComputation.GetCqi(txmode, 32);
    }
}
