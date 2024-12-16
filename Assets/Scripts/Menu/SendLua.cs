using UnityEngine;



public class SendLua : MonoBehaviour
{

    public PlinkSSH plinkInterface;
    public JsonToLua jsonToLua;
    public string luaData;
    private bool startLuaSend;

    public string commandTelnet = "telnet 127.0.0.1 12345";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLuaSend = false;
    }

    // Update is called once per frame
    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 1;


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
        luaData = jsonToLua.luaData;
        if (startLuaSend) 
        {
            SendLuaPlink();
        }
    }


    public void ToggleLuaSendState()
    {
        startLuaSend = !startLuaSend;
    }

    void SendLuaPlink()
    {
        plinkInterface.SendCommand("echo \"" + luaData + "\" | " + commandTelnet);
    }
}
