using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLine : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public string targetTag = "Obstacle"; 
    public LineRenderer lineRendererG;
    public LineRenderer lineRendererR;
    public bool collision = false;

    void Start()
    {
        LineInitialisation(lineRendererG);
        LineInitialisation(lineRendererR);
    }

    void Update()
    {
        if (objectA != null && objectB != null)
        {
            UpdateLinePosition(lineRendererG);
            UpdateLinePosition(lineRendererR);

            CheckLineCollision(); 

            if (collision)
            {
                lineRendererR.enabled = true;
                lineRendererG.enabled = false;
            }
            else
            {
                lineRendererR.enabled = false;
                lineRendererG.enabled = true;
            }
        }
    }


    void UpdateLinePosition(LineRenderer line)
    {
        line.SetPosition(0, objectA.transform.position);
        line.SetPosition(1, objectB.transform.position);
    }

    void LineInitialisation (LineRenderer line)
    {
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
    }
    void CheckLineCollision()
    {
        Vector3 direction = objectB.transform.position - objectA.transform.position;
        Ray ray = new Ray(objectA.transform.position, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude);
        collision = false;

        foreach (RaycastHit raycastHit in hits)
        {
            if (raycastHit.collider.CompareTag(targetTag))
            {
                collision = true;
                break; 
            }
        }
    }
}
