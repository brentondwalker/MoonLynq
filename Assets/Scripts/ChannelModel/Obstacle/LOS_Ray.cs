using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LOS_Ray : MonoBehaviour
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

    public double totalLossForward;
    public double totalLossReverse;

    public double totalLossForwardInDB;
    public double totalLossReverseInDB;


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

        hitReverse = hitReverse.OrderBy(hit => Vector3.Distance(objectA.transform.position, hit.point)).ToArray();


        collision = false;
        totalLossForward = 1.0;
        totalLossReverse = 1.0;
        totalLossForwardInDB = 0.0;
        totalLossReverseInDB = 0.0;

        int hitCountForward = hitForward.Length;

        if (hitCountForward > 0) { collision = true; }

        for (int i = 0; i < hitCountForward; i++)
        {
            if (hitForward[i].collider != null)
            {
                Collider obstacleHit = hitForward[i].collider;
                totalLossForward *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, UeInfo.frequency, objectA.transform.position, objectB.transform.position);
                totalLossForwardInDB = PowerCalculator.linearToDb(totalLossForward);
            }
        }

        int hitCountReverse = hitReverse.Length;

        if (hitCountReverse > 0) { collision = true; }

        for (int i = 0; i < hitCountReverse; i++)
        {
            if (hitReverse[i].collider != null)
            {
                Collider obstacleHit = hitReverse[i].collider;           
                totalLossReverse *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, UeInfo.frequency, objectB.transform.position, objectA.transform.position);
                totalLossReverseInDB = PowerCalculator.linearToDb(totalLossReverse);
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
                hitInfo += "Forward Hit: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }

        // Log reverse hits
        foreach (RaycastHit hit in hitReverse)
        {
            if (hit.collider != null)
            {
                hitInfo += "Reverse Hit: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }
    }
}
