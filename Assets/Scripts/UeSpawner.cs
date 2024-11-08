using UnityEngine;
using System.Collections;

public class UeSpawner : MonoBehaviour
{
    public GameObject uePrefab;
    public int ueCount = 5;  
    public Vector3 originLocation = new Vector3(0, 0, 0);  
    public GameObject parentObject;
    public int randomSpawnRange = 100;

    public UeInfo UeInfo;

    private int currentSpawnCount = 0;

    void Start()
    {
        if (parentObject != null)
        {
            StartCoroutine(SpawnUes());
        }
        else
        {
            Debug.LogError("Parent object not assigned!");
        }
    }

    IEnumerator SpawnUes()
    {
        while (currentSpawnCount < ueCount)
        {
            Vector3 spawnPosition = new Vector3(0, 0, 0);
            Vector3 spawnPositionDeviation = new Vector3(
                Random.Range(-randomSpawnRange, randomSpawnRange), 
                0, 
                Random.Range(-randomSpawnRange, randomSpawnRange)
            );
            spawnPosition += originLocation;

            GameObject ueInstance = Instantiate(uePrefab, spawnPosition, Quaternion.identity);
            ueInstance.transform.SetParent(parentObject.transform);
            ueInstance.transform.localPosition = spawnPosition + spawnPositionDeviation;
            ueInstance.transform.localScale = uePrefab.transform.localScale;


           
            currentSpawnCount++;
            ueInstance.name = "Ue_" + (currentSpawnCount+2049);
            UeInfo.ueId = currentSpawnCount + 2049;
            yield return null;
        }
        Debug.Log("All UEs spawned.");
    }
}