using UnityEngine;

public class UeStatusDisplay : MonoBehaviour
{
    public float rotationSpeed = 45f;
    public bool status = false;

    private Renderer objectRenderer;
    public Material materialIdle;
    public Material materialActive;
    public Material materialHover;

    private bool isHovering = false;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        StatusEvent.OnStatusChanged += OnOtherStatusChanged;
    }

    private void OnDestroy()
    {
        StatusEvent.OnStatusChanged -= OnOtherStatusChanged;
    }

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            isHovering = true;
        }
        else
        {
            isHovering = false;
        }

        if (status)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            objectRenderer.material = isHovering ? materialHover : materialActive;
        }
        else
        {
            transform.Rotate(Vector3.up, rotationSpeed * 0.3f * Time.deltaTime);
            objectRenderer.material = isHovering ? materialHover : materialIdle;
        }

        if (Input.GetMouseButtonDown(0) && isHovering)
        {
            ToggleStatus();
        }
    }

    private void ToggleStatus()
    {
        status = !status;
        if (status)
        {
            StatusEvent.InvokeStatusChanged(this);
        }
    }

    private void OnOtherStatusChanged(UeStatusDisplay sender)
    {
        if (sender != this && sender.status)
        {
            status = false;
        }
    }
}

public static class StatusEvent
{
    public delegate void StatusChangedHandler(UeStatusDisplay sender);
    public static event StatusChangedHandler OnStatusChanged;

    public static void InvokeStatusChanged(UeStatusDisplay sender)
    {
        if (OnStatusChanged != null)
        {
            OnStatusChanged(sender);
        }
    }
}