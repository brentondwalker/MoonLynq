using UnityEngine;
using System;

public class EnbInfo : MonoBehaviour
{
    public int enbId = 1;
    public double txPower = 40;
    public Vector3 enbMobiltiyLocal;
    public string enbMobilityInfo;
    public GameObject enbObject;

    private void Update()
    {
        enbMobiltiyLocal = transform.localPosition;
        enbMobilityInfo = enbId.ToString() + ": " + Math.Round(enbMobiltiyLocal.x, 1) + " " + -Math.Round(enbMobiltiyLocal.z, 1) + " " + Math.Round(enbMobiltiyLocal.y, 1);
    }

}
