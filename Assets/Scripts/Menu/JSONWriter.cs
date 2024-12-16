using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JSONWriter : MonoBehaviour
{
    [SerializeField] public string filename = "emulation_data.json";

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 0.01f;

    public InputFieldManager inputHARQLossRate;

    public string jsonData;
    public bool writeJsonToLocal = true;

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
        jsonData = WriteEmulationData();
    }

    public string WriteEmulationData()
    {
        EmulationData emulationData = new EmulationData
        {
            HARQ_loss_rate = inputHARQLossRate.inputNumber
        };

        string json = JsonConvert.SerializeObject(emulationData, Formatting.Indented);

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        if (writeJsonToLocal)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(json);
            }

            Debug.Log("JSON data written to: " + filePath);
        }

        return json;
    }
}

[System.Serializable]
public class EmulationData
{
    public float HARQ_loss_rate;
}
