using UnityEngine;
using System;

public class GnbInfo : MonoBehaviour
{
    public int gnbId = 1;
    public double txPower = 40;
    public Vector3 gnbMobiltiyLocal;
    public string gnbMobilityInfo;


    private void Update()
    {
        gnbMobiltiyLocal = transform.localPosition;
        gnbMobilityInfo = gnbId.ToString() + ": " + Math.Round(gnbMobiltiyLocal.x, 1) + " " + -Math.Round(gnbMobiltiyLocal.z, 1) + " " + Math.Round(gnbMobiltiyLocal.y, 1);
    }

}
