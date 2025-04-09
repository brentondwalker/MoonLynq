using System;
using System.Collections.Generic;
using UnityEngine;

public class DielectricObstacleLoss : MonoBehaviour
{
    public bool enableDielectricLoss = true;
    public bool enableReflectionLoss = true;
    public string obstacleTag = "Obstacle";  
    public MaterialProperties mediumMaterial;
    private int intersectionComputationCount = 0;
    private int intersectionCount = 0;
    public float distanceConvert = 1;

    public List<Vector3> hitPointsPosition = new List<Vector3>();


    public bool useSegmentedRaycasts = false;
    public int segments = 1;

    void Start()
    {
        distanceConvert = 1/ScenarioScale.staticScale;
    }

    void OnDestroy()
    {
    }

    double ComputeDielectricLoss(MaterialProperties material, float frequency, double distance)
    {
        float lossTangent = GetDielectricLossTangent(material, frequency);
        float propagationSpeed = GetPropagationSpeed(material);
        //double delta = Math.Atan(lossTangent);
        float k = 2 * Mathf.PI * frequency / propagationSpeed;
        double factor = Math.Exp(-1 * lossTangent * (k * distance));

        //Debug.Log("Dielectric loss factor: " + factor);
        //Debug.Log("Distance: " + distance);

        return Mathf.Clamp((float)factor, 0, 1);
    }

    double ComputeReflectionLoss(MaterialProperties incidentMaterial, MaterialProperties refractiveMaterial, float angle)
    {

        float n1 = GetRefractiveIndex(incidentMaterial);
        float n2 = GetRefractiveIndex(refractiveMaterial);
        float sinAngle = Mathf.Sin(angle);
        float cosAngle = Mathf.Cos(angle);

        float k = Mathf.Sqrt(1 - Mathf.Pow(n1 / n2 * sinAngle, 2));
        double rs = Math.Pow((n1 * cosAngle - n2 * k) / (n1 * cosAngle + n2 * k), 2);
        double rp = Math.Pow((n1 * k - n2 * cosAngle) / (n1 * k + n2 * cosAngle), 2);
        double reflectance = (rs + rp) / 2;

        //Debug.Log("Angle: " + angle);
        //Debug.Log("Transmittance: " + (1 - reflectance));

        return 1 - reflectance;
    }

    public double ComputeObjectLoss(Collider obstacleHit, float frequency, Vector3 transmissionPosition, Vector3 receptionPosition)
    {
        double totalLoss = 1;
        Vector3 direction = (receptionPosition - transmissionPosition).normalized;


        //intersectionComputationCount++;
        //intersectionCount++;

        


        MaterialProperties material = obstacleHit.gameObject.GetComponent<MaterialProperties>();

        if (useSegmentedRaycasts)
        {

            List<RaycastHit> hits = PerformSegmentedRaycasts(obstacleHit, transmissionPosition, receptionPosition, segments);

            for (int i = 0; i < hits.Count - 1; i += 2)
            {
                RaycastHit hit1 = hits[i];
                RaycastHit hit2 = hits[i + 1];

                if (enableDielectricLoss)
                {
                    double intersectionDistance = Vector3.Distance(hit1.point, hit2.point) * distanceConvert;
                    totalLoss *= ComputeDielectricLoss(material, frequency, intersectionDistance);
                }

                if (enableReflectionLoss)
                {
                    float angle = Vector3.Angle(hit1.normal, -direction);
                    totalLoss *= ComputeReflectionLoss(mediumMaterial, material, angle * Mathf.Deg2Rad);
                }
            }
        }
        else 
        {
            if (material == null)
            {
                //Debug.LogWarning("Material not found!");
            }

            Ray rayF = new Ray(transmissionPosition, direction);
            RaycastHit hitTransmission;
            obstacleHit.Raycast(rayF, out hitTransmission, Vector3.Distance(transmissionPosition, receptionPosition));

            Ray rayB = new Ray(receptionPosition, -direction);
            RaycastHit hitReception;
            obstacleHit.Raycast(rayB, out hitReception, Vector3.Distance(receptionPosition, transmissionPosition));

            if (enableDielectricLoss)
            {
                double intersectionDistance = Vector3.Distance(hitTransmission.point, hitReception.point) * distanceConvert;
                //Debug.Log("hitTransmission.point: " + hitTransmission.point);
                //Debug.Log("hitReception.point: " + hitReception.point);
                //Debug.Log("IntersectionDistance: " + intersectionDistance);
                totalLoss *= ComputeDielectricLoss(material, frequency, intersectionDistance);
            }

            if (enableReflectionLoss)
            {
                float angle = Vector3.Angle(hitTransmission.normal, -direction);
                totalLoss *= ComputeReflectionLoss(mediumMaterial, material, angle * Mathf.Deg2Rad);
            }
        }

        return totalLoss;
    }

    float GetDielectricLossTangent(MaterialProperties material, float frequency)
    {
        return material.GetDielectricLossTangent(frequency);
    }

    float GetPropagationSpeed(MaterialProperties material)
    {
        return material.GetPropagationSpeed();
    }

    float GetRefractiveIndex(MaterialProperties material)
    {
        return material.GetRefractiveIndex();
    }

    public List<RaycastHit> PerformSegmentedRaycasts(Collider obstacleHit, Vector3 startPosition, Vector3 endPosition, int segments)
    {
        List<RaycastHit> raycastResults = new List<RaycastHit>();
        hitPointsPosition.Clear(); 

        Vector3 direction = (endPosition - startPosition).normalized;


        Ray rayF = new Ray(startPosition, direction);
        RaycastHit hitTransmission;
        obstacleHit.Raycast(rayF, out hitTransmission, Vector3.Distance(startPosition, endPosition));

        Ray rayB = new Ray(endPosition, -direction);
        RaycastHit hitReception;
        obstacleHit.Raycast(rayB, out hitReception, Vector3.Distance(endPosition, startPosition));

        //float distanceTotal = Vector3.Distance(startPosition,endPosition);
        float distance = Vector3.Distance(hitTransmission.point, hitReception.point);
        float segmentLength = distance / segments;
        float epsilon = 0.002f;

        for (int i = 0; i <= segments; i++)
        {
            Vector3 segmentPoint = hitTransmission.point + direction * (i * segmentLength);

            Ray rayForward = new Ray(segmentPoint, direction);
            RaycastHit hitForward;
            bool forwardHit = obstacleHit.Raycast(rayForward, out hitForward, distance);

            Ray rayBackward = new Ray(segmentPoint, -direction);
            RaycastHit hitBackward;
            bool backwardHit = obstacleHit.Raycast(rayBackward, out hitBackward, distance);

            if (forwardHit && backwardHit && i != 0 && i != segments)
            {
                if (Vector3.Distance(hitForward.point, hitBackward.point) > epsilon)
                {
                    if (!IsDuplicatePoint(raycastResults, hitForward.point, epsilon))
                    {
                        raycastResults.Add(hitForward);
                        hitPointsPosition.Add(hitForward.point); 
                    }
                    if (!IsDuplicatePoint(raycastResults, hitBackward.point, epsilon))
                    {
                        raycastResults.Add(hitBackward);
                        hitPointsPosition.Add(hitBackward.point);
                    }
                }
            }
            else
            {
                if (i == 0)
                {
                    raycastResults.Add(hitTransmission);
                    hitPointsPosition.Add(hitTransmission.point);
                }
                else if (i == segments)
                {
                    raycastResults.Add(hitReception);
                    hitPointsPosition.Add(hitReception.point); 
                }
            }
        }

        raycastResults.Sort((hit1, hit2) => Vector3.Distance(startPosition, hit1.point).CompareTo(Vector3.Distance(startPosition, hit2.point)));
        hitPointsPosition.Sort((point1, point2) => Vector3.Distance(startPosition, point1).CompareTo(Vector3.Distance(startPosition, point2)));

        return raycastResults;
    }


    private bool IsDuplicatePoint(List<RaycastHit> hits, Vector3 point, float epsilon)
    {
        foreach (var hit in hits)
        {
            if (Vector3.Distance(hit.point, point) < epsilon)
            {
                return true; 
            }
        }
        return false;
    }
}