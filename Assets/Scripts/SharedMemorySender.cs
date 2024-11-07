using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SharedMemorySender : MonoBehaviour
{
    public TxPower TxPower;
    private const string SharedMemoryName = "Omnetpp_SharedMemoryReceive";
    private const int BufferSize = 4096; 

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr MapViewOfFile(IntPtr hMapObject, uint dwDesiredAccess, uint dwOffsetHigh, uint dwOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private IntPtr sharedMemoryHandle;
    private IntPtr sharedMemoryView;

    void Start()
    {
        sharedMemoryHandle = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)BufferSize, SharedMemoryName);
        if (sharedMemoryHandle == IntPtr.Zero)
        {
            Debug.LogError("Failed to create shared memory.");
            return;
        }

        sharedMemoryView = MapViewOfFile(sharedMemoryHandle, 0xF001F, 0, 0, (uint)BufferSize);
        if (sharedMemoryView == IntPtr.Zero)
        {
            Debug.LogError("Failed to map view of shared memory.");
            CloseHandle(sharedMemoryHandle);
            return;
        }
    }

    private void Update()
    {
        //SendData("ueTxPower: "+ TxPower.uePower*TxPower.uePowerFactor +", " + "eNodeBtxPower: "+TxPower.eNodeBtxPower*TxPower.eNodeBtxPowerFactor);
        if (false)
        {
            SendData("txPower1: 5\n" +
                     "txPower2050: 3\n" +
                     "txPower2049: " + TxPower.uePower * TxPower.uePowerFactor + "\n" +
                     "txPower2049->1: " + 20.5 * TxPower.uePowerFactor + "\n" +
                     "txPower2049->2: " + 30.5 + "\n" +
                     "txPower1->2050: " + 1 + "\n" +
                     "txPower1->2051: " + 2 + "\n" +
                     "txPower1->2052: " + 3 + "\n" +
                     "txPower1->2053: " + 4 + "\n" +
                     "txPower1->2054: " + 5 + "\n" +
                     "txPower1->2055: " + 6 + "\n" +
                     "txPower2->2050: " + 61 + "\n" +
                     "txPower2->2051: " + 62 + "\n" +
                     "txPower2->2052: " + 63 + "\n" +
                     "txPower2->2053: " + 64 + "\n" +
                     "txPower2->2054: " + 65 + "\n" +
                     "txPower2->2055: " + 66 + "\n" +
                     "txPower1->2049: " + 0 + "\n" +
                     "txPower2->2049: " + 60 + "\n");
        }
        if (true)
        {
            SendData("txPower1: 5\n" +
                     "txPower2050: 3\n" +
                     "txPower2049: " + TxPower.uePower * TxPower.uePowerFactor + "\n" +
                     "txPower2049->1: " + 20.5 * TxPower.uePowerFactor + "\n" +
                     "txPower2049->2: " + 30.5 + "\n" +
                     "txPower2->2050: " + 1 + "\n" +
                     "txPower2->2051: " + 2 + "\n" +
                     "txPower2->2052: " + 3 + "\n" +
                     "txPower2->2053: " + 4 + "\n" +
                     "txPower2->2054: " + 5 + "\n" +
                     "txPower2->2055: " + 6 + "\n" +
                     "txPower1->2050: " + 61 * TxPower.uePowerFactor + "\n" +
                     "txPower1->2051: " + 62 * TxPower.uePowerFactor + "\n" +
                     "txPower1->2052: " + 63 * TxPower.uePowerFactor + "\n" +
                     "txPower1->2053: " + 64 * TxPower.uePowerFactor + "\n" +
                     "txPower1->2054: " + 65 * TxPower.uePowerFactor + "\n" +
                     "txPower1->2055: " + 66 * TxPower.uePowerFactor + "\n" +
                     "txPower2->2049: " + 0 + "\n" +
                     "txPower1->2049: " + 60 * TxPower.uePowerFactor + "\n");
        }
    }

    public void SendData(string data)
    {
        if (sharedMemoryView != IntPtr.Zero)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            if (dataBytes.Length < BufferSize)
            {
                Marshal.Copy(dataBytes, 0, sharedMemoryView, dataBytes.Length);
                Marshal.WriteByte(sharedMemoryView, dataBytes.Length, 0);
            }
            else
            {
                Debug.LogError("Data too large for shared memory.");
            }
        }
        else
        {
            Debug.LogError("Shared memory is not initialized.");
        }
    }

    void OnApplicationQuit()
    {
        if (sharedMemoryView != IntPtr.Zero)
        {
            UnmapViewOfFile(sharedMemoryView);
            sharedMemoryView = IntPtr.Zero;
        }
        if (sharedMemoryHandle != IntPtr.Zero)
        {
            CloseHandle(sharedMemoryHandle);
            sharedMemoryHandle = IntPtr.Zero;
        }
    }
}