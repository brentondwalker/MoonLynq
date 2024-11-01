using UnityEngine;
using System.Diagnostics;
using System.IO;

public class SCPFile : MonoBehaviour
{
    public InputFieldManager interfaceInput;

    public string remoteFilePath = "Nairong@pc32.filab.uni-hannover.de:/mnt/Unity/PCAP/";
    public string localFilePath = @"C:\Users\Bude\source\repos\PCAP_Test\PCAP";

    private string fileName;

    void Start()
    {
        fileName = interfaceInput.inputText;
        remoteFilePath = remoteFilePath + fileName + "*";
    }

    public void StartSCP ()
    {
        ExecuteSCPCommand(remoteFilePath, localFilePath);
    }

    void ExecuteSCPCommand(string remotePath, string localPath)
    {
        string scpCommand = $"scp {remotePath} {localPath}";

        UnityEngine.Debug.Log(scpCommand);

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {scpCommand}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false, 
            CreateNoWindow = true
        };


        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            UnityEngine.Debug.Log($"Output: {output}");
            UnityEngine.Debug.Log($"Error: {error}");

            if (process.ExitCode != 0)
            {
                UnityEngine.Debug.LogError("SCP command failed with exit code " + process.ExitCode);
            }
            else
            {
                UnityEngine.Debug.Log("SCP command completed successfully.");
            }
        }
    }
}