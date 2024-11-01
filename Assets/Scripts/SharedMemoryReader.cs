using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using UnityEngine;

public class SharedMemoryReader : MonoBehaviour
{
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;
    private const int maxRetries = 3; 
    private const float retryDelay = 1.0f; 

    public string data;

    void Start()
    {
        InitializeSharedMemory();
    }

    private void InitializeSharedMemory()
    {
        int attempts = 0;
        while (attempts < maxRetries)
        {
            try
            {
                mmf = MemoryMappedFile.OpenExisting("SharedMemory_MacDelayUL");
                accessor = mmf.CreateViewAccessor(0, 256, MemoryMappedFileAccess.Read);
                Debug.Log("Shared memory opened successfully.");
                return; 
            }
            catch (Exception ex)
            {
                attempts++;
                Debug.LogError($"Error initializing shared memory: {ex.Message}. Attempt {attempts}/{maxRetries}.");
                System.Threading.Thread.Sleep((int)(retryDelay * 1000));
            }
        }

        Debug.LogError("Failed to initialize shared memory after multiple attempts.");
    }

    void Update()
    {
        if (accessor != null)
        {
            byte[] buffer = new byte[256];
            accessor.ReadArray(0, buffer, 0, buffer.Length);
            data = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            if (!string.IsNullOrEmpty(data))
            {
                //Debug.Log("Received from OMNeT++: " + data);
            }
        }
    }

    void OnDestroy()
    {
        if (accessor != null)
        {
            accessor.Dispose();
            Debug.Log("Accessor disposed.");
        }
        if (mmf != null)
        {
            mmf.Dispose();
            Debug.Log("Memory mapped file disposed.");
        }
    }
}