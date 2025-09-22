using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using TMPro;

public class DataDisplay : MonoBehaviour
{
    public TextMeshProUGUI ueTxPowerText; 
    public TextMeshProUGUI MacLayerDelayText; 
    private string filePath1 = "C:\\Users\\Bude\\simu5g\\omnetpp-6.0.3-windows-x86_64\\omnetpp-6.0.3\\samples\\test_5g\\src\\udtest\\powertestoutput.txt";
    private string filePath2 = "C:\\Users\\Bude\\simu5g\\omnetpp-6.0.3-windows-x86_64\\omnetpp-6.0.3\\samples\\test_5g\\src\\udtest\\MacLayerDelay_output.txt";

    public TextMeshProUGUI SMMacLayerDelay;
    public SharedMemoryReader SharedMemoryReader;

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    public float updateInterval = 0.2f;


    void Update()
    {
        /*
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }

        string smMacLayerDelay = SharedMemoryReader.data;
        SMMacLayerDelay.text = smMacLayerDelay;
        */
    }


    void CustomUpdate()
    {
        if (File.Exists(filePath1) && File.Exists(filePath2))
        {
            string ueTxPower = File.ReadAllText(filePath1); 
            string MacLayerDelay = File.ReadAllText(filePath2); 

            ueTxPowerText.text = ueTxPower;
            MacLayerDelayText.text = MacLayerDelay;
        }
        else
        {
            Debug.LogError("�ļ�·�������ڣ�");
        }
    }
}