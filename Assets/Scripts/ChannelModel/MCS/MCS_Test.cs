using UnityEngine;
using static LteMCS;

public class MCS_Test : MonoBehaviour
{
    public LteAMC lteAmc;
    public LteMCS lteMcs;
    public CQI_Test cqiTest;
    public UeInfo ueInfo;

    public int cqi;
    public int iTbs;
    public int tbs;
    public Modulation mod;
    public int bandwidth;

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
        cqi = cqiTest.cqi;
        int numBands = ueInfo.numBands;
        tbs = lteAmc.ComputeBitsOnNRbs(cqi,1,1,true);
        iTbs = lteAmc.GetItbsPerCqi(cqi, true);
        CQIelem entry = lteMcs.CQITable[cqi];
        mod = entry.ModulationType;
        bandwidth = lteAmc.BandWidthComputation(numBands,tbs,mod);
    }

}
