using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualLine : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public string targetTag = "Obstacle"; 
    public LineRenderer lineRendererG;
    public LineRenderer lineRendererR;
    public bool collision = false;

    public RaycastHit[] hitForward;
    public RaycastHit[] hitReverse;
    public string hitInfo;


    public DielectricObstacleLoss DielectricObstacleLoss;
    public UeInfo UeInfo;

    public double totalLoss;


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
        hitInfo = string.Empty;
        hitForward = new RaycastHit[0];
        hitReverse = new RaycastHit[0];

        Vector3 directionF = objectB.transform.position - objectA.transform.position;
        Ray rayF = new Ray(objectA.transform.position, directionF);

        hitForward = Physics.RaycastAll(rayF, directionF.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitForward = hitForward.OrderBy(hit => Vector3.Distance(objectA.transform.position, hit.point)).ToArray();

        Vector3 directionR = objectA.transform.position - objectB.transform.position;
        Ray rayR = new Ray(objectB.transform.position, directionR);

        hitReverse = Physics.RaycastAll(rayR, directionR.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitReverse = hitReverse.OrderBy(hit => Vector3.Distance(objectB.transform.position, hit.point)).ToArray();

        collision = false;
        totalLoss = 1.0;  

        int maxHits = Mathf.Max(hitForward.Length, hitReverse.Length);

        if (maxHits > 0) { collision = true; }

        for (int i = 0; i < maxHits; i++)
        {
            if (i < hitForward.Length && hitForward[i].collider != null)
            {
                RaycastHit hitF = hitForward[i];

                int reverseIndex = hitReverse.Length - 1 - i;
                if (reverseIndex >= 0 && reverseIndex < hitReverse.Length && hitReverse[reverseIndex].collider != null)
                {
                    RaycastHit hitR = hitReverse[reverseIndex];
                    if (hitF.collider.gameObject == hitR.collider.gameObject)
                    {
                        totalLoss *= DielectricObstacleLoss.ComputeObjectLoss(hitF, hitR, UeInfo.frequency, objectA.transform.position, objectB.transform.position);
                    }
                }
            }
        }

        LogHitInfo();
    }

    void LogHitInfo()
    {
        // Log forward hits
        foreach (RaycastHit hit in hitForward)
        {
            if (hit.collider != null)
            {
                hitInfo += "Forward Hit - GameObject: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }

        // Log reverse hits
        foreach (RaycastHit hit in hitReverse)
        {
            if (hit.collider != null)
            {
                hitInfo += "Reverse Hit - GameObject: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }
    }
}
