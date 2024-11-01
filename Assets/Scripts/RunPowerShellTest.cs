using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RunPowerShellTest : MonoBehaviour
{

    public Button myButton;
    private Process powerShellProcess;
    private string commandFilePath;
    public string path;

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);

        StartPowerShellProcess(path);
    }

    void OnApplicationQuit()
    {
        if (powerShellProcess != null && !powerShellProcess.HasExited)
        {
            powerShellProcess.Kill();
        }

        if (File.Exists(commandFilePath))
        {
            File.Delete(commandFilePath);
        }
    }

    void StartPowerShellProcess(string path)
    {
        commandFilePath = Path.Combine(Application.dataPath, path);

        if (File.Exists(commandFilePath))
        {
            File.Delete(commandFilePath);
        }
        File.WriteAllText(commandFilePath, "");

        string script = $@"
while ($true) {{
    if (Test-Path '{commandFilePath}') {{
        Get-Content '{commandFilePath}' | ForEach-Object {{
            Write-Output $_
            Invoke-Expression $_
        }}
        Clear-Content '{commandFilePath}'
    }}
    Start-Sleep -Seconds 1
}}";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoExit -Command \"{script}\"",
            UseShellExecute = true, 
            CreateNoWindow = false  
        };

        powerShellProcess = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        powerShellProcess.Start();
    }

    void OnButtonClick()
    {
        if (File.Exists(commandFilePath))
        {
            File.AppendAllText(commandFilePath, "ping 192.168.0.1" + System.Environment.NewLine);
        }
    }
}
