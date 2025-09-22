using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NodeManager : MonoBehaviour
{
    private GameObject[] nodeObjects;
    public string ulInfo = "";
    public string dlInfo = "";
    public string mobilityInfo = "";
    public GenericNodeBase nodeSelect = null;

    void Start()
    {
        StartCoroutine(FindNode());
        nodeObjects = GameObject.FindGameObjectsWithTag("MLNode");
        
    }

    private IEnumerator FindNode()
    {
        yield return new WaitForSeconds(2f);
        GameObject[] nodeObjects = GameObject.FindGameObjectsWithTag("MLNode");
    }

        private void Update()
    {
        nodeSelect = null;

        GameObject[] nodeObjects = GameObject.FindGameObjectsWithTag("MLNode");
        if (nodeObjects != null)
        {
            ulInfo = "";
            dlInfo = "";
            mobilityInfo = "";
            foreach (GameObject nodeObject in nodeObjects)
            {
                GenericNodeBase nodeBase = nodeObject.GetComponent<GenericNodeBase>();

                if (nodeBase != null)
                {
                    ulInfo += nodeBase.ulInfo + "\n";
                    dlInfo += nodeBase.dlInfo + "\n";
                    mobilityInfo += nodeBase.mobilityInfo + "\n";
                    if (nodeBase.isSelect) { nodeSelect = nodeBase; }
                }
                else
                {
                    Debug.LogWarning("No GenericNodeBase component found on " + nodeObject.name);
                }
            }

            //Debug.Log(ulInfo + dlInfo);

        }
    }
}