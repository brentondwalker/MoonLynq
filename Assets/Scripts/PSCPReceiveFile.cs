using System.Diagnostics;
using UnityEngine;
using System.Threading.Tasks;

public class PSCPReceiveFile : MonoBehaviour
{
    public string privateKeyPath = "\"C:\\Users\\Bude\\.ssh\\id_rsa_putty.ppk\"";
    public string pscpPath = "C:\\Users\\Bude\\Emulator Test\\Assets\\pscp.exe";
    public string localFilePath = @"C:\Users\Bude\source\repos\PCAP_Test\";
    public string sshUser = "Nairong";
    public string sshHost = "pc32.filab.uni-hannover.de";
    public string remoteFilePath = "/mnt/Unity/PCAP/";

    public InputFieldManager interfaceInput;
    private string fileName;

    void Start()
    {
        fileName = interfaceInput.inputText;
        remoteFilePath = remoteFilePath + fileName + "*";

    }
    public void FileDownload()
    {
        Task.Run(() => ReceiveFileAsync(remoteFilePath,localFilePath));
    }

    public void TransferFile()
    {
        string command = $"\"{pscpPath}\" -i \"{privateKeyPath}\" \"{localFilePath}\" {sshUser}@{sshHost}:{remoteFilePath}";

        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/c \"{command}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("Command Output:\n" + output);
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError("Command Error:\n" + error);
        }
    }


    public async Task ReceiveFileAsync(string remoteFilePath, string localFilePath)
    {
        string command = $"\"{pscpPath}\" -i \"{privateKeyPath}\" {sshUser}@{sshHost}:{remoteFilePath} \"{localFilePath}\"";

        UnityEngine.Debug.Log(command);

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            process.Exited += (sender, args) => tcs.SetResult(true);

            await tcs.Task;

            // Read initial output and error
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Await process exit
            await tcs.Task;

            // Read remaining output and error
            output += process.StandardOutput.ReadToEnd();
            error += process.StandardError.ReadToEnd();

            UnityEngine.Debug.Log("Command Output:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError("Command Error:\n" + error);
            }
        }
    }
}