using System;
using UnityEngine;
using static VehicleBlindSpotDetector;

public class VehicleDataCommunicator : MonoBehaviour
{
    public VehicleBlindSpotDetector selfBlindSpot;
    public ChannelModelManager channelModelManager;
    private BlindSpotInfo[] selfBlindSpotInfo;
    private UeBase[] ueBases;
    private int[] cqi;
    public int cqiThreshold = 5;

    public bool enableDataCommunication = true;

    public string blindSpotInfoDisplay = "";

    private void Start()
    {
        selfBlindSpotInfo = selfBlindSpot.blindSpotInfos;
    }
    private void Update()
    {

        selfBlindSpotInfo = selfBlindSpot.blindSpotInfos;
        BlindSpotInfo[] targerBlindSpot = new BlindSpotInfo[selfBlindSpotInfo.Length];
        ueBases = channelModelManager.ueBases;
        cqi = channelModelManager.cqi;

        for (int i = 0; i < selfBlindSpotInfo.Length; i++)
        {
            if (selfBlindSpotInfo[i].lastVisibleTimeViaCommunication < selfBlindSpotInfo[i].lastVisibleTime) selfBlindSpotInfo[i].lastVisibleTimeViaCommunication = selfBlindSpotInfo[i].lastVisibleTime;
        }

        if (enableDataCommunication)
        {
            for (int j = 0; j < ueBases.Length; j++)
            {
                if (cqi[j] < cqiThreshold) break;
                targerBlindSpot = ueBases[j].extraModule.vehicleBlindSpotDetector.blindSpotInfos;
                for (int i = 0; i < selfBlindSpotInfo.Length; i++)
                {
                    if (selfBlindSpotInfo[i].lastVisibleTimeViaCommunication < targerBlindSpot[i].lastVisibleTimeViaCommunication) selfBlindSpotInfo[i].lastVisibleTimeViaCommunication = targerBlindSpot[i].lastVisibleTimeViaCommunication;
                }
            }
        }

        for (int i = 0; i < selfBlindSpotInfo.Length; i++)
        {
            selfBlindSpotInfo[i].TimeSinceLastVisibleTime = Math.Max(Time.time - selfBlindSpotInfo[i].lastVisibleTimeViaCommunication - Time.deltaTime, 0);
        }
        blindSpotInfoDisplay = GetBlindSpotInfoDisplay();
    }

    public string GetBlindSpotInfoDisplay()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var info in selfBlindSpotInfo)
        {
            sb.AppendLine($"NID: {info.nodeId}, PID: {info.pointId}, " +
                          $"VS: {info.isVisible}, LVT: {info.lastVisibleTimeViaCommunication:F2}, TSLVT: : {info.TimeSinceLastVisibleTime:F2}");
        }

        return sb.ToString();
    }
}
