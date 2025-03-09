using UnityEngine;
using TMPro;

public class DropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public string currentMode;

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        currentMode = dropdown.options[dropdown.value].text;
    }

    void OnDropdownValueChanged(int index)
    {
        currentMode = dropdown.options[dropdown.value].text;
    }
}