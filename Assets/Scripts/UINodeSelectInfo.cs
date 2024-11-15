using TMPro;
using UnityEngine;
using System;



public class UINodeSelectInfo : MonoBehaviour
{
    public UeManager ueManager;
    private UeInfo selectUe;
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
        selectUe = null;
        displayInfo = string.Empty;
        display.SetActive(false);
        if (ueManager.ueSelect != null) 
        {
            display.SetActive(true);
            selectUe = ueManager.ueSelect;
            displayInfo =
                            "<b>Nodetype:</b> Ue\n" +
                            "<b>NodeId:</b> " + selectUe.GetUeId() + "\n" +
                            "<b>Position:</b> " +
                                Math.Round(selectUe.prefabPosition.x, 1) + " " +
                                -Math.Round(selectUe.prefabPosition.z, 1) + " " +
                                Math.Round(selectUe.prefabPosition.y, 1) + "\n" +
                            "<b>TxPower:</b> " + Math.Round(selectUe.GetTxPower(), 1) + "dBm" + "\n" + 
                            "<b>Upload:</b> " + selectUe.GetUeId() + "->" + selectUe.TargetGnb.gnbId +
                                " TxPower: " + Math.Round(selectUe.GetTxPowerUl(), 1) + "dBm" + " Loss: " + Math.Round(selectUe.VisualLine.totalLossForwardInDB,1) + "dBm" + "\n" +
                            "<b>Download:</b> " + selectUe.TargetGnb.gnbId + "->" + selectUe.ueId +
                                " TxPower: " + Math.Round(selectUe.GetTxPowerDl(), 1) + "dBm" + " Loss " + Math.Round(selectUe.VisualLine.totalLossReverseInDB,1) + "dBm" + "\n";
            nodeInfoDisplay.text = displayInfo;
        }
    }
}
