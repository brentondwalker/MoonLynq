using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using TMPro;

[System.Serializable]
public class DataRecord
{
    public float t;  
    public float throughput;
    public double bler;
    public int iTbs;

    public DataRecord(float time, float throughputValue, double blerValue, int tbsIndex)
    {
        t = time;
        throughput = throughputValue;
        bler = blerValue;
        iTbs = tbsIndex;
    }
}

public class DataRecorder : MonoBehaviour
{
    public float recordInterval = 0.1f;  
    public float recordDuration = 10.0f; 
    public float elapsedTime = 0.0f;    
    private bool isRecording = false;

    private List<DataRecord> recordedData = new List<DataRecord>();
    public MCS_Test mcs;
    public IsError isError;

    public InputFieldManager intervalInput;
    public InputFieldManager durationInput;

    public TMP_Text showElapesdTime;
    public TMP_Text showPkts;
    public TMP_Text showBLER;
    public TMP_Text showITBS;

    public ConvertJsonToLua jsonToLua;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isRecording)
        {
            StartCoroutine(RecordData());
        }


        showElapesdTime.text = elapsedTime.ToString("000.0");
        if (mcs != null) showPkts.text = mcs.pkts.ToString("0.0");
        if (isError != null) showBLER.text = isError.bler.ToString("0.000");
        if (mcs != null) showITBS.text = mcs.iTbs.ToString("0");
        recordInterval = intervalInput.inputNumber;
        recordDuration = durationInput.inputNumber;
        
    }

    IEnumerator RecordData()
    {
        isRecording = true;
        elapsedTime = 0f;
        recordedData.Clear();

        while (elapsedTime < recordDuration)
        {
            recordedData.Add(new DataRecord(elapsedTime, mcs.pkts, isError.bler, mcs.iTbs));

            yield return new WaitForSeconds(recordInterval);
            elapsedTime += recordInterval;
        }

        Debug.Log("Recording finished!");
        
        SaveToJson();

        isRecording = false;
    }


    void SaveToJson()
    {
        string json = JsonConvert.SerializeObject(recordedData, Formatting.Indented);
        string filePath = Path.Combine(Application.streamingAssetsPath, "emulation_data_record.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to: " + filePath);
        jsonToLua.WriteLua(json, "emulation_data_record.lua");
    }
}