using UnityEngine;

public class CQI_Test : MonoBehaviour
{
    public CQI_Computaion cqiComputation;
    public LteSINR lteSINR;
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
        meanSNR = cqiComputation.MeanSnr(lteSINR.GetSINR(true));
        cqi = cqiComputation.GetCqi(txmode, meanSNR);
        //cqi = cqiComputation.GetCqi(txmode, 32);
    }
}
