using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static TransmissionParameterManager;

public class LOS_Ray : MonoBehaviour
{
    //public GameObject objectA;
    //public GameObject objectB;
    public string targetTag = "Obstacle"; 
    public LineRenderer lineRendererG;
    public LineRenderer lineRendererR;
    public bool collision = false;

    public RaycastHit[] hitForward;
    public RaycastHit[] hitReverse;

    public string hitInfo;


    public DielectricObstacleLoss DielectricObstacleLoss;
    public UeBase UeBase;

    public double totalLossForward;
    public double totalLossReverse;

    public double totalLossForwardInDB;
    public double totalLossReverseInDB;

    private Vector3 start = Vector3.zero;
    private Vector3 end = Vector3.zero;

    public double distance = 0.0;

    public float lineWidth = 0.5f;

    

    void Start()
    {
        LineInitialisation(lineRendererG);
        LineInitialisation(lineRendererR);
    }

    void Update()
    {

        

            if (collision)
            {
                UpdateLinePosition(lineRendererR, start, end);
                UpdateLinePosition(lineRendererG, Vector3.zero, Vector3.zero);
            }
            else
            {
                UpdateLinePosition(lineRendererG, start, end);
                UpdateLinePosition(lineRendererR, Vector3.zero, Vector3.zero);
            }
        IdleDetection();
    }

    float lastUpdateTime = -1f;

    void IdleDetection()
    {
        if (Time.time - lastUpdateTime > 0.5)
        {
            UpdateLinePosition(lineRendererG, Vector3.zero, Vector3.zero);
            UpdateLinePosition(lineRendererR, Vector3.zero, Vector3.zero);
        }
    }


    void UpdateLinePosition(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void LineInitialisation (LineRenderer line)
    {
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
    }
    public double GetLosLoss(bool isUpload, TransmissionParameter parameter)
    {
        
        lastUpdateTime = parameter.lastUpdateTime;

        
        hitInfo = string.Empty;
        hitForward = new RaycastHit[0];
        hitReverse = new RaycastHit[0];

        start = parameter.positionA;
        end = parameter.positionB;

        distance = Vector3.Distance(start, end);
        Vector3 directionF = end - start;
        Ray rayF = new Ray(start, directionF);

        hitForward = Physics.RaycastAll(rayF, directionF.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitForward = hitForward.OrderBy(hit => Vector3.Distance(start, hit.point)).ToArray();

        Vector3 directionR = start - end;
        Ray rayR = new Ray(end, directionR);

        hitReverse = Physics.RaycastAll(rayR, directionR.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitReverse = hitReverse.OrderBy(hit => Vector3.Distance(start, hit.point)).ToArray();


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
                totalLossForward *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, UeBase.radioParameters.frequency, start, end);
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
                totalLossReverse *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, UeBase.radioParameters.frequency, end, start);
                totalLossReverseInDB = PowerCalculator.linearToDb(totalLossReverse);
            }
        }
        LogHitInfo();

        if (isUpload) return totalLossForwardInDB;
        else return totalLossReverseInDB;
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
