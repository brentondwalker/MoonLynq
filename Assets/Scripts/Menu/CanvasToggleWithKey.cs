using UnityEngine;

public class CanvasToggleWithKey : MonoBehaviour
{
    public Canvas targetCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCanvasAndChildren();
        }
    }
    void ToggleCanvas()
    {
        if (targetCanvas != null)
        {
            targetCanvas.enabled = !targetCanvas.enabled;
        }
    }
    void ToggleCanvasAndChildren()
    {
        if (targetCanvas != null)
        {
            bool currentState = targetCanvas.enabled;
            targetCanvas.enabled = !currentState;
            foreach (Canvas childCanvas in targetCanvas.GetComponentsInChildren<Canvas>(true))
            {
                childCanvas.enabled = !currentState;
            }
        }
        else
        {
            Debug.LogWarning("Canvas “˝”√Œ¥…Ë÷√£°");
        }
    }

}