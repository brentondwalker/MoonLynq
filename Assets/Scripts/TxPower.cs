using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // 引入文件操作命名空间

public class TxPower : MonoBehaviour
{
    public VisualLine line;
    public double uePower = 26;
    public double uePowerFactor = 1;
    public double eNodeBtxPower = 40;
    public double eNodeBtxPowerFactor = 1;
    private string filePath = "C:\\Users\\Bude\\simu5g\\omnetpp-6.0.3-windows-x86_64\\omnetpp-6.0.3\\samples\\test_5g\\src\\udtest\\powertest.txt"; 

    void Start()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, ""); 
        }
    }

    void Update()
    {
        if (line != null)
        {
            if (line.collision)
            {
                uePowerFactor = 0.05;
                eNodeBtxPowerFactor = 0.05;
            }
            else
            {
                uePowerFactor = 1;
                eNodeBtxPowerFactor = 1;
            }

            double result = uePower * uePowerFactor; 
            LogResultToFile(result); 
        }
    }

    void LogResultToFile(double result)
    {
        File.WriteAllText(filePath, result.ToString()); 
    }
}