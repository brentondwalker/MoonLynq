using TMPro;
using UnityEngine;
using static LteMCS;
using System;

public class MCS_Test : MonoBehaviour
{
    public LteAMC lteAmc;
    public LteMCS lteMcs;
    public CQI_Base cqiBase;
    public GenericNodeBase nodeBase;

    public int cqi;
    public int iTbs;
    public int tbs;
    public Modulation mod;
    public float throughput;

    public int packetLength = 1344;
    public float pkts = 0f;

    void Start()
    {

    }

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 0.1f;



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
        cqi = cqiBase.cqi;
        int numBands = nodeBase.radioParameters.numBands;
        int numLayers = nodeBase.radioParameters.numLayers;
        tbs = lteAmc.ComputeBitsOnNRbs(cqi,numLayers,1,true);
        iTbs = lteAmc.GetItbsPerCqi(cqi, true);
        CQIelem entry = lteMcs.CQITable[cqi];
        mod = entry.ModulationType;
        throughput = lteAmc.ThroughputComputation(numBands,numLayers,tbs);
        pkts = throughput / 8 / packetLength;
    }

}
