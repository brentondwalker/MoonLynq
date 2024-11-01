using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OmnetDataWriter : MonoBehaviour
{
    public GameObject targetObject; 
    public string filePath = "Assets/position.txt";

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    public float updateInterval = 1;

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
        WritePosition();
    }

    void WritePosition()
    {
        if (targetObject != null)
        {
            Vector3 position = targetObject.transform.localPosition;

            string positionString = $"{position.x} {-position.z} {0}";

            File.WriteAllText(filePath, positionString);
        }
    }
}
