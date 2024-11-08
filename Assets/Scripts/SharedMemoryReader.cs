using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Collections;
using UnityEngine;

public class SharedMemoryReader : MonoBehaviour
{
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;

    public string data;

    // 用于跟踪内存是否已经打开
    private bool isMemoryInitialized = false;

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 3;

    void Start()
    {
        StartCoroutine(InitializeSharedMemory());
    }

    private IEnumerator InitializeSharedMemory()
    {
        if (isMemoryInitialized)
        {
            Debug.Log("Shared memory is already initialized.");
            yield break; 
        }

        try
        {
            mmf = MemoryMappedFile.OpenExisting("Omnetpp_SharedMemorySend");
            accessor = mmf.CreateViewAccessor(0, 256, MemoryMappedFileAccess.Read);
            Debug.Log("Shared memory opened successfully.");
            isMemoryInitialized = true; 
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing shared memory: {ex.Message}");
            yield break;
        }

        yield break;
    }

    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }

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

    void CustomUpdate()
    {
        if (!isMemoryInitialized) 
        {
            StartCoroutine(InitializeSharedMemory());
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