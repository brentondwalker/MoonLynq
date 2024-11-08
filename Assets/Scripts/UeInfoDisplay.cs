using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UeInfoDisplay : MonoBehaviour
{

    public TextMeshProUGUI ueInfoDisplay;
    public UeInfo UeInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ueInfoDisplay.text = "Ue_" + UeInfo.GetUeId().ToString() + "\n"
                            + UeInfo.ulInfo + "\n"
                            + UeInfo.dlInfo;
    }
}
