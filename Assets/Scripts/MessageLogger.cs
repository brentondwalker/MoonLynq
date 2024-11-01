using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
        return logMessages;
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
}
