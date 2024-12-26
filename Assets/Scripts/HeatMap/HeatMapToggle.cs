using UnityEngine;
using UnityEngine.UI;

public class HeatMapToggle : MonoBehaviour
{
    public Toggle toggle;
    public GameObject heatMapPlane;

    private void Start()
    {
        heatMapPlane.SetActive(toggle.isOn);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        heatMapPlane.SetActive(isOn);
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
