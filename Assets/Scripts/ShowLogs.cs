using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShowLogs : MonoBehaviour
{
    public TMP_InputField logField;
    public MessageLogger messageLogger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<string> log = messageLogger.GetLog();
        string logText = string.Join("\n", log);
        logField.text = logText;
    }
}
