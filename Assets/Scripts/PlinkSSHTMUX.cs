using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PlinkSSHTMUX : MonoBehaviour
{
    private Process sshProcess;
    private StreamWriter inputStream;
    private StreamReader outputStream;

    public InputCommand inputCommand;

    public string sshPath = "\"C:\\Users\\Bude\\Emulator Test\\Assets\\plink.exe\"";
    public string privateKeyPath = "\"C:\\Users\\Bude\\.ssh\\id_rsa_putty.ppk\"";
    public string sshUser = "Nairong";
    public string sshHost = "pc32.filab.uni-hannover.de";
    public string tmuxSessionName = "unity_tmux_session";

    void Start()
    {
    }

    void OnDestroy()
    {
        if (sshProcess != null && !sshProcess.HasExited)
        {
            sshProcess.Kill();
        }
    }

    public void StartSSHSession()
    {
        string command = $"-ssh -i {privateKeyPath} {sshUser}@{sshHost}";

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
            }
        };

        sshProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.LogError($"SSH Error: {e.Data}");
            }
        };

        sshProcess.Start();
        sshProcess.BeginOutputReadLine();
        sshProcess.BeginErrorReadLine();

        inputStream = sshProcess.StandardInput;

        // Start the tmux session
        SendCommand($"tmux new-session -d -s {tmuxSessionName}");
        UnityEngine.Debug.Log("Started tmux session");
    }

    public void SendCommand(string command)
    {
        if (inputStream != null)
        {
            // Send the command to the tmux session
            string tmuxCommand = $"tmux send-keys -t {tmuxSessionName} \"{command}\" C-m";
            inputStream.WriteLine(tmuxCommand);
            inputStream.Flush();
            UnityEngine.Debug.Log($"Sent command to tmux: {command}");
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