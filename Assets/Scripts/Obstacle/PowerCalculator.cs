using UnityEngine;
using System;

public class PowerCalculator : MonoBehaviour
{
    public static double dBToLinear(double dbValue)
    {
        return Math.Pow(10, dbValue / 10);
    }

    public static double linearToDb(double linearValue)
    {
        return 10 * Math.Log10(linearValue);
    }

    public static double CalculateTxPowerAfterLoss(double txPowerDb, double totalLossLinear)
    {
        double txPowerLinear = dBToLinear(txPowerDb);
        double finalPowerLinear = txPowerLinear * totalLossLinear;
        return linearToDb(finalPowerLinear);
    }
}