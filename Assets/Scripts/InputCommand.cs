using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputCommand : MonoBehaviour
{
    public TMP_InputField command;
    public string commandText;
    private void OnCommandChanged(string newCommand)
    {
        Debug.Log("New command set: " + newCommand);
        
        commandText = newCommand;
        command.text = commandText;
    }
    private void Start()
    {
        if (command != null)
        {
            command.onEndEdit.AddListener(OnCommandChanged);
        }
    }
}