using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MobilitySender : MonoBehaviour
{
    private const string SharedMemoryName = "Omnetpp_MobilityReceive";
    private const int BufferSize = 4096;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr MapViewOfFile(IntPtr hMapObject, uint dwDesiredAccess, uint dwOffsetHigh, uint dwOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private IntPtr mobilityHandle;
    private IntPtr mobilityView;


    void Start()
    {
        mobilityHandle = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, (uint)BufferSize, SharedMemoryName);
        if (mobilityHandle == IntPtr.Zero)
        {
            Debug.LogError("Failed to create shared memory.");
            return;
        }

        mobilityView = MapViewOfFile(mobilityHandle, 0xF001F, 0, 0, (uint)BufferSize);
        if (mobilityView == IntPtr.Zero)
        {
            Debug.LogError("Failed to map view of shared memory.");
            CloseHandle(mobilityHandle);
            return;
        }
    }

    private void Update()
    {

        SendData("2049: 370 345 0\n"
                    +"2050: 50 50 0\n"
                    +"2051: 30 90 0\n"
                    +"2052: 150 250 0\n"
                    +"2053: 250 50 0\n");


    }

    public void SendData(string data)
    {
        if (mobilityView != IntPtr.Zero)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            if (dataBytes.Length < BufferSize)
            {
                Marshal.Copy(dataBytes, 0, mobilityView, dataBytes.Length);
                Marshal.WriteByte(mobilityView, dataBytes.Length, 0);
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
        if (mobilityView != IntPtr.Zero)
        {
            UnmapViewOfFile(mobilityView);
            mobilityView = IntPtr.Zero;
        }
        if (mobilityHandle != IntPtr.Zero)
        {
            CloseHandle(mobilityHandle);
            mobilityHandle = IntPtr.Zero;
        }
    }
}