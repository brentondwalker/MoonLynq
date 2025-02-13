using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas[] canvases;

    private void Start()
    {
        ShowCanvas(0);
    }

    public void ShowCanvas(int canvasIndex)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (i == canvasIndex)
            {
                canvases[i].enabled = true;  
            }
            else
            {
                canvases[i].enabled = false; 
            }
        }
    }
}
