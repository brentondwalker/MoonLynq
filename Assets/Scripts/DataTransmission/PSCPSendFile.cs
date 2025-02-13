using System.Diagnostics;
using UnityEngine;
using System.Threading.Tasks;

public class PSCPSendFile : MonoBehaviour
{
    public string privateKeyPath = "\"C:\\Users\\Bude\\.ssh\\id_rsa_putty.ppk\"";
    public string pscpPath = "C:\\Users\\Bude\\Emulator Test\\Assets\\pscp.exe";
    public string localFilePath = @"C:\Users\Bude\Emulator Test\Assets\StreamingAssets\emulation_data_throughput.lua";
    public string sshUser = "Nairong";
    public string sshHost = "pc7.filab.uni-hannover.de";
    public string remoteFilePath = "/mnt/";


    public JsonToLua jsonToLua;


    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 1f;


    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }
    void CustomUpdate()
    {

        //Task.Run(() => TransferFilesAsync());
    }

    public void RunAsyncTask()
    {
        Task.Run(() => TransferFilesAsync());
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

    public async Task TransferFilesAsync()
    {
        jsonToLua.ConvertJsonToLua();

        string command = $"\"{pscpPath}\" -i \"{privateKeyPath}\" \"{localFilePath}\" {sshUser}@{sshHost}:{remoteFilePath}";

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

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            UnityEngine.Debug.Log("Command Output:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError("Command Error:\n" + error);
            }
        }
    }
}