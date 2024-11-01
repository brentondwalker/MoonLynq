using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RunPowerShell : MonoBehaviour
{

    private Process powerShellProcess;
    private string commandFilePath;
    public InputCommand inputCommand;
    public string SSH;
    public string node;
    public string command;
    private string plinkPath;

    void Start()
    {
        string projectRootPath = Directory.GetParent(Application.dataPath).FullName;
        plinkPath = Path.Combine(projectRootPath, "plink.exe");
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

    public void StartPowerShellProcess()
    {
        if (node != null)
        {
            commandFilePath = Path.Combine(Application.dataPath, node + ".txt");

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
    }

    private void PowerShellCommand(string command)
    {
        // 向命令文件中写入命令
        if (File.Exists(commandFilePath))
        {
            File.AppendAllText(commandFilePath, command + System.Environment.NewLine);
        }
    }

    public void PowerShellManualCommand()
    {

        string SSHKeyPath = @"C:\Users\Bude\.ssh\id_rsa";
        string plinkPath = @"C:\Users\Bude\Emulator Test\Assets\plink.exe";
        string commandToExecute = $"& {plinkPath} -ssh -i {SSHKeyPath} Nairong@pc32.filab.uni-hannover.de ping 127.0.0.1";

        commandToExecute = inputCommand.commandText;

        //& "C:\Users\Bude\Emulator Test\Assets\plink.exe" -ssh -i "C:\Users\Bude\.ssh\id_rsa_putty.ppk" Nairong@pc32.filab.uni-hannover.de ping 127.0.0.1
        //& "C:\Users\Bude\Emulator Test\Assets\plink.exe" -ssh -i "C:\Users\Bude\.ssh\id_rsa_putty.ppk" Nairong@pc32.filab.uni-hannover.de ps aux | grep ping
        PowerShellCommand(commandToExecute);

    }
    public void PowerShellSSH()
    {
        PowerShellCommand(SSH);
    }
}
