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

    public DataRecord(float time, float value)
    {
        t = time;
        throughput = value;
    }
}

public class DataRecorder : MonoBehaviour
{
    public float recordInterval = 0.1f;  
    public float recordDuration = 10.0f; 
    public float elapsedTime = 0.0f;    
    public float externalValue = 0.0f;   
    private bool isRecording = false;

    private List<DataRecord> recordedData = new List<DataRecord>();
    public MCS_Test mcs;

    public InputFieldManager intervalInput;
    public InputFieldManager durationInput;

    public TMP_Text showElapesdTime;
    public TMP_Text showCurrentValue;

    public ConvertJsonToLua jsonToLua;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isRecording)
        {
            StartCoroutine(RecordData());
        }


        externalValue = mcs.pkts;

        showElapesdTime.text = elapsedTime.ToString("000.0");
        showCurrentValue.text = externalValue.ToString("0.0");
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
            recordedData.Add(new DataRecord(elapsedTime, externalValue));
            Debug.Log($"Recorded: Time={elapsedTime}, ExternalValue={externalValue}");

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
        string filePath = Path.Combine(Application.streamingAssetsPath, "emulation_data_throughput.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to: " + filePath);
        jsonToLua.WriteLua(json, "emulation_data_throughput.lua");
    }
}