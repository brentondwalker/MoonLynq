using UnityEngine;

public class LteHARQ : MonoBehaviour
{
    public IsError isError;

    public double ErrorRateFrame = 0;
    public double PktErrorRateFrame = 0;
    public MCS_Test mcsTest;

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
        int cqi = mcsTest.cqiTest.cqi;
        int numBands = mcsTest.ueInfo.numBands;
        ErrorRateFrame = isError.GetErrorRate(cqi, numBands, mcsTest.cqiTest.snrv);
        PktErrorRateFrame = isError.GetPktErrorRate(1344, ErrorRateFrame, mcsTest.throughput);
        //PktErrorRateFrame = isError.GetPktErrorRate(1344, 0.5, mcsTest.throughput);
    }
}
