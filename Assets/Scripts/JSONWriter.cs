using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JSONWriter : MonoBehaviour
{
    [SerializeField] public string filename = "emulation_data.json";
    public string filenameClient = "client_data.json";

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 1;

    public InputFieldManager inputLatency;
    public InputFieldManager inputRate;
    public InputFieldManager inputRateClient;

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
        WriteEmulationData();
        WriteClientData();
    }

    void WriteEmulationData()
    {
        EmulationData emulationData = new EmulationData
        {
            latency = new float[] { inputLatency.inputNumber, inputLatency.inputNumber },
            rate = new float[] { inputRate.inputNumber, inputRate.inputNumber }
        };

        string json = JsonConvert.SerializeObject(emulationData, Formatting.Indented);

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

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

    void WriteClientData()
    {
        ClientData clientData = new ClientData
        {
            rateClient = inputRateClient.inputNumber
        };

        string json = JsonConvert.SerializeObject(clientData, Formatting.Indented);

        string filePath = Path.Combine(Application.streamingAssetsPath, filenameClient);

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(json);
        }

        Debug.Log("Client JSON data written to: " + filePath);
    }
}

[System.Serializable]
public class EmulationData
{
    public float[] latency;
    public float[] rate;
}

[System.Serializable]
public class ClientData
{
    public float rateClient;
}