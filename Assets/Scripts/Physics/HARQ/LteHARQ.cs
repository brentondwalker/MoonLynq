using UnityEngine;

public class LteHARQ : MonoBehaviour
{
    public IsError isError;

    public double per = 0;
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
        int cqi = mcsTest.cqiBase.cqi;
        int numBands = mcsTest.ueBase.ueParameters.numBands;
        per = isError.GetErrorRate(cqi, numBands, mcsTest.cqiBase.snrv);
    }
}
