using UnityEngine;

public class NRHARQ : MonoBehaviour
{
    public IsError isError;

    public double per = 0;
    public NRMCS_Test mcsTest;
    public bool enablePer = true;

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
        int numBands = mcsTest.ueBase.radioParameters.numBands;
        if (enablePer) per = isError.GetErrorRate(cqi, numBands, mcsTest.cqiBase.snrv);
        else per = 0;
    }
}
