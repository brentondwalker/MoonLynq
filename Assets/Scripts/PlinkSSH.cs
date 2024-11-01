using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PlinkSSH : MonoBehaviour
{
    private Process sshProcess;
    private StreamWriter inputStream;
    private StreamReader outputStream;

    public InputCommand inputCommand;
    public MessageLogger messageLogger;

    public string sshPath = "\"C:\\Users\\Bude\\Emulator Test\\Assets\\plink.exe\"";
    public string privateKeyPath = "\"C:\\Users\\Bude\\.ssh\\id_rsa_putty.ppk\"";
    public string sshUser = "Nairong";
    public string sshHost = "pc32.filab.uni-hannover.de";

    public bool enterMoonGenDirectory = false; 

    void Start()
    {
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
        string command = $"-ssh -i \"{privateKeyPath}\" {sshUser}@{sshHost}";

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
                UnityEngine.Debug.Log($"SSH Output: {e.Data}");
                messageLogger.LogMessage(e.Data);
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
