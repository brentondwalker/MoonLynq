using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UeManager : MonoBehaviour
{
    private GameObject[] ueObjects;
    public string ulInfo = "";
    public string dlInfo = "";
    public string mobilityInfo = "";

    void Start()
    {
        StartCoroutine(FindUe());
        ueObjects = GameObject.FindGameObjectsWithTag("Ue");
        
    }

    private IEnumerator FindUe()
    {
        yield return new WaitForSeconds(2f);
        GameObject[] ueObjects = GameObject.FindGameObjectsWithTag("Ue");
    }

        private void Update()
    {
        GameObject[] ueObjects = GameObject.FindGameObjectsWithTag("Ue");
        if (ueObjects != null)
        {
            ulInfo = "";
            dlInfo = "";
            mobilityInfo = "";
            foreach (GameObject ueObject in ueObjects)
            {
                UeInfo ueInfo = ueObject.GetComponent<UeInfo>();

                if (ueInfo != null)
                {
                    ulInfo += ueInfo.ulInfo + "\n";
                    dlInfo += ueInfo.dlInfo + "\n";
                    mobilityInfo += ueInfo.mobilityInfo + "\n";
                }
                else
                {
                    Debug.LogWarning("No UeInfo component found on " + ueObject.name);
                }
            }

            //Debug.Log(ulInfo + dlInfo);

        }
    }
}