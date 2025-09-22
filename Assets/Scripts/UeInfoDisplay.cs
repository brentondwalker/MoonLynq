using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UeInfoDisplay : MonoBehaviour
{

    public TextMeshProUGUI ueInfoDisplay;
    public GenericNodeBase nodeBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ueInfoDisplay.text = "Ue_" + nodeBase.nodeId.ToString() + "\n"
                            + nodeBase.ulInfo + "\n"
                            + nodeBase.dlInfo;
    }
}
