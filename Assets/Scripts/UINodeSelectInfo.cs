using TMPro;
using UnityEngine;
using System;



public class UINodeSelectInfo : MonoBehaviour
{
    public NodeManager nodeManager;
    private GenericNodeBase selectNode;
    public string displayInfo;
    public TextMeshProUGUI nodeInfoDisplay;
    public GameObject display;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        selectNode = null;
        displayInfo = string.Empty;
        display.SetActive(false);
        if (nodeManager.nodeSelect != null) 
        {
            display.SetActive(true);
            selectNode = nodeManager.nodeSelect;
            /*
            displayInfo =
                            "<b>Nodetype:</b> Ue\n" +
                            "<b>NodeId:</b> " + selectNode.nodeId + "\n" +
                            "<b>Position:</b> " +
                                Math.Round(selectNode.prefabPosition.x, 1) + " " +
                                -Math.Round(selectNode.prefabPosition.z, 1) + " " +
                                Math.Round(selectNode.prefabPosition.y, 1) + "\n" +
                            "<b>TxPower:</b> " + Math.Round(selectNode.txPowerUl, 1) + "dBm" + "\n" + 
                            "<b>Upload:</b> " + selectNode.nodeId + "->" + selectNode.TargetNode.nodeId +
                                " TxPower: " + Math.Round(selectNode.txPowerUl, 1) + "dBm" + " Loss: " + Math.Round(selectNode.LOS_Ray.GetLosLoss(true, selectNode.transmissionParameters[0]),1) + "dBm" + "\n" +
                            "<b>Download:</b> " + selectNode.TargetNode.nodeId + "->" + selectNode.nodeId +
                                " TxPower: " + Math.Round(selectNode.txPowerDl, 1) + "dBm" + " Loss " + Math.Round(selectNode.LOS_Ray.GetLosLoss(false, selectNode.transmissionParameters[0]), 1) + "dBm" + "\n";
                                */
            displayInfo = "<b>Nodetype:</b> Ue\n";
            displayInfo += "<b>NodeId:</b> " + selectNode.nodeId + "\n";
            displayInfo += "<b>Position:</b> ";
            displayInfo += Math.Round(selectNode.prefabPosition.x, 1) + " ";
            displayInfo += -Math.Round(selectNode.prefabPosition.z, 1) + " ";
            displayInfo += Math.Round(selectNode.prefabPosition.y, 1) + "\n";
            displayInfo += "<b>TxPower:</b> " + Math.Round(selectNode.txPowerUl, 1) + "dBm" + "\n";
            displayInfo += "<b>Upload:</b> " + selectNode.nodeId + "->" + selectNode.TargetNode.nodeId +"\n"; 
            displayInfo += "<b>Distance:</b> " + Math.Round(selectNode.LOS_Ray.distance) + "\n";
            displayInfo += " TxPower: " + Math.Round(selectNode.txPowerUl, 1) + "dBm" + " Loss: " + Math.Round(selectNode.LOS_Ray.GetLosLoss(true, selectNode.transmissionParameters[0]), 1) + "dBm" + "\n";
            displayInfo += "<b>Download:</b> " + selectNode.TargetNode.nodeId + "->" + selectNode.nodeId;
            displayInfo += " TxPower: " + Math.Round(selectNode.txPowerDl, 1) + "dBm" + " Loss " + Math.Round(selectNode.LOS_Ray.GetLosLoss(false, selectNode.transmissionParameters[0]), 1) + "dBm" + "\n";

            nodeInfoDisplay.text = displayInfo;
        }
    }
}
