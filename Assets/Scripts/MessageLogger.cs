using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class MessageLogger : MonoBehaviour
{
    public Scrollbar scrollbar;
    private List<string> logMessages;
    private bool scroll;

    void Start()
    {
        logMessages = new List<string>();
        scroll = false;
    }

    private void Update()
    {
        if (scroll) {
            ScrollToBottom();
            scroll = false;
        }

    }

    public void LogMessage(string message)
    {
        logMessages.Add(message);
        scroll = true;
    }

    public List<string> GetLog()
    {
        int count = logMessages.Count;
        if (count > 100)
        {
            return logMessages.GetRange(count - 100, 100);
        }
        return new List<string>(logMessages);
    }

    public void ClearLog()
    {
        logMessages.Clear();
    }

    public void ScrollToBottom()
    {
        if (scrollbar != null)
        {
            scrollbar.value = 1f;
        }
    }

    public void SaveLogToFile()
    {
        string path = Path.Combine(Application.dataPath, "log.txt");
        try
        {
            File.WriteAllLines(path, logMessages);
            Debug.Log($"Log saved to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save log: {e.Message}");
        }
    }
}
