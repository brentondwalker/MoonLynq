using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UeInfoDisplay : MonoBehaviour
{

    public TextMeshProUGUI ueInfoDisplay;
    public UeBase UeBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ueInfoDisplay.text = "Ue_" + UeBase.ueId.ToString() + "\n"
                            + UeBase.ulInfo + "\n"
                            + UeBase.dlInfo;
    }
}
