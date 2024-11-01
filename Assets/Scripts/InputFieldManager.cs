using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField input;
    public string inputText;
    public float inputNumber;
    public bool inputBoolean;
    public string valueName;


    public enum ValueType
    {
        Number,
        Text
    }

    public ValueType valueType;

    private void OnCommandChanged(string newInput)
    {
        switch (valueType)
        {
            case ValueType.Number:
                if (float.TryParse(newInput, out float result))
                {
                    inputNumber = result;
                    input.text = newInput;
                    Debug.Log(valueName + ": " + "New float value set: " + newInput);
                }
                else
                {
                    Debug.LogError("Invalid input value! The input should be float.");
                }
                break;
            case ValueType.Text:
                Debug.Log(valueName + ": " + "New value set: " + newInput);

                inputText = newInput;
                input.text = newInput;
                break;
        }
        Debug.Log(valueName + ": " + "New value set: " + newInput);

        inputText = newInput;
        input.text = inputText;
    }
    private void Start()
    {
        if (input != null)
        {
            input.onEndEdit.AddListener(OnCommandChanged);
        }
        switch (valueType)
        {
            case ValueType.Number:
                input.text = inputNumber.ToString();
                break;
            case ValueType.Text:
                input.text = inputText;
                break;
        }
    }
}
