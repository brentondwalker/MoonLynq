using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleLos : MonoBehaviour
{
    public string targetTag = "Obstacle"; 
    public bool collision = false;

    private RaycastHit[] hitForward;
    private RaycastHit[] hitReverse;

    private string hitInfo;

    public DielectricObstacleLoss DielectricObstacleLoss;

    private double totalLossForward;
    private double totalLossReverse;

    private double totalLossForwardInDB;
    private double totalLossReverseInDB;


    void Start()
    {

    }

    void Update()
    {

    }



    public string CheckLineCollision(float frequency, Vector3 positionA, Vector3 positionB)
    {

        hitForward = new RaycastHit[0];
        hitReverse = new RaycastHit[0];


        Vector3 directionF = positionB - positionA;
        Ray rayF = new Ray(positionA, directionF);

        hitForward = Physics.RaycastAll(rayF, directionF.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitForward = hitForward.OrderBy(hit => Vector3.Distance(positionA, hit.point)).ToArray();

        Vector3 directionR = positionA - positionB;
        Ray rayR = new Ray(positionB, directionR);

        hitReverse = Physics.RaycastAll(rayR, directionR.magnitude)
                     .Where(hit => hit.collider.CompareTag(targetTag))
                     .ToArray();

        hitReverse = hitReverse.OrderBy(hit => Vector3.Distance(positionA, hit.point)).ToArray();


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
                totalLossForward *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, frequency, positionA, positionB);
                totalLossForwardInDB = PowerCalculator.linearToDb(totalLossForward);
                //Debug.Log(totalLossForward);
            }
        }

        int hitCountReverse = hitReverse.Length;

        if (hitCountReverse > 0) { collision = true; }

        for (int i = 0; i < hitCountReverse; i++)
        {
            if (hitReverse[i].collider != null)
            {
                Collider obstacleHit = hitReverse[i].collider;           
                totalLossReverse *= DielectricObstacleLoss.ComputeObjectLoss(obstacleHit, frequency, positionB, positionA);
                totalLossReverseInDB = PowerCalculator.linearToDb(totalLossReverse);
            }
        }
        return LogHitInfo();
    }

    string LogHitInfo()
    {
        hitInfo = string.Empty;

        foreach (RaycastHit hit in hitForward)
        {
            if (hit.collider != null)
            {
                hitInfo += "Forward Hit: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }
        foreach (RaycastHit hit in hitReverse)
        {
            if (hit.collider != null)
            {
                hitInfo += "Reverse Hit: " + hit.collider.gameObject.name + ", Position: " + hit.point.ToString() + "\n";
            }
        }

        return hitInfo;
    }

    public double getTotalLossForwardInDB() {  return totalLossForwardInDB; }
    public double getTotalLossReverseInDB() { return totalLossReverseInDB; }
}
