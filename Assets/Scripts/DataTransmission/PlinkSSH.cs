using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;

public class PlinkSSH : MonoBehaviour
{
    private Process sshProcess;
    private StreamWriter inputStream;
    private StreamReader outputStream;

    public InputCommand inputCommand;
    public MessageLogger messageLogger;
    public MGMessageManager messageManager;

    public string sshPath = "\"C:\\Users\\Bude\\Emulator Test\\Assets\\plink.exe\"";
    public string privateKeyPath = "\"C:\\Users\\Bude\\.ssh\\id_rsa_putty.ppk\"";
    public string sshUser = "Nairong";
    public string sshHost = "pc32.filab.uni-hannover.de";

    public bool enterMoonGenDirectory = false;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();


    void Start()
    {
    }

    private void Update()
    {
        while (logQueue.TryDequeue(out string logMessage))
        {
            UnityEngine.Debug.Log(logMessage);
            messageLogger.LogMessage(logMessage);
            if (messageManager != null)
            {
                if (logMessage.Contains("[Pkt]"))
                {
                    messageManager.HandlePktMessage(logMessage);
                    UnityEngine.Debug.Log("PKT message handled");
                }
                else if (logMessage.Contains("[RLC]"))
                {
                    messageManager.HandleRlcMessage(logMessage);
                    UnityEngine.Debug.Log("RLC message handled");
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (sshProcess != null && !sshProcess.HasExited)
        {
            sshProcess.Kill();
        }
    }

    public void StartSSHSession()
    {
        string command = $"-ssh -i \"{privateKeyPath}\" {sshUser}@{sshHost} -batch";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "plink.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        sshProcess = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        sshProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                //UnityEngine.Debug.Log($"SSH Output: {e.Data}");
                //messageLogger.LogMessage(e.Data);

                logQueue.Enqueue($"SSH Output: {e.Data}");
            }
        };

        sshProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.LogError($"SSH Error: {e.Data}");
                messageLogger.LogMessage(e.Data);

            }
        };

        sshProcess.Start();
        sshProcess.BeginOutputReadLine();
        sshProcess.BeginErrorReadLine();

        inputStream = sshProcess.StandardInput;

        if (enterMoonGenDirectory) { SendCommand("cd MoonGen"); }
    }

    public void StartSSHTunnel()
    {
        string command = $"-ssh -L 12350:127.0.0.1:12350 -i \"{privateKeyPath}\" {sshUser}@{sshHost}";
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = sshPath,
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        sshProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
        sshProcess.OutputDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log($"SSH Output: {e.Data}");
        };
        sshProcess.ErrorDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogError($"SSH Error: {e.Data}");
        };

        sshProcess.Start();
        sshProcess.BeginOutputReadLine();
        sshProcess.BeginErrorReadLine();
    }

    public void SendCommand(string command)
    {
        if (inputStream != null)
        {
            inputStream.WriteLine(command);
            inputStream.Flush();
            UnityEngine.Debug.Log($"Sent command: {command}");
        }
        else
        {
            UnityEngine.Debug.LogError("Input stream is not initialized.");
        }
    }
    public void SendCommandInputField()
    {
        if (inputCommand != null)
        {
            SendCommand(inputCommand.commandText);
        }
        else
        {
            UnityEngine.Debug.LogError("Cant find input field.");
        }
    }
}
