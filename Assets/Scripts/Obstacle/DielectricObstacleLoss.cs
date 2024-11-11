using System;
using UnityEngine;

public class DielectricObstacleLoss : MonoBehaviour
{
    public bool enableDielectricLoss = true;
    public bool enableReflectionLoss = true;
    public string obstacleTag = "Obstacle";  
    public MaterialProperties mediumMaterial;
    private int intersectionComputationCount = 0;
    private int intersectionCount = 0;
    public float distanceConvert = 10;

    void Start()
    {

    }

    void OnDestroy()
    {
    }

    double ComputeDielectricLoss(MaterialProperties material, float frequency, float distance)
    {
        float lossTangent = GetDielectricLossTangent(material, frequency);
        float propagationSpeed = GetPropagationSpeed(material);
        double delta = Math.Atan(lossTangent);
        float k = 2 * Mathf.PI * frequency / propagationSpeed;
        double factor = Math.Exp(-2 * delta * (k * distance));
        Debug.Log("Dielectric loss factor: " + factor);
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
        return 1 - reflectance;
    }

    public double ComputeObjectLoss(RaycastHit hit1, RaycastHit hit2, float frequency, Vector3 transmissionPosition, Vector3 receptionPosition)
    {
        double totalLoss = 1;
        Vector3 direction = (receptionPosition - transmissionPosition).normalized;
        intersectionComputationCount++;
        intersectionCount++;
        MaterialProperties material = hit1.collider.gameObject.GetComponent<MaterialProperties>();

        if (enableDielectricLoss)
        {
            float intersectionDistance = Vector3.Distance(hit1.point, hit2.point) * distanceConvert;
            //Debug.Log("IntersectionDistance: " + intersectionDistance);
            totalLoss *= ComputeDielectricLoss(material, frequency, intersectionDistance);
        }

        if (enableReflectionLoss)
        {
            float angle = Vector3.Angle(hit1.normal, -direction);
            totalLoss *= ComputeReflectionLoss(mediumMaterial, material, angle * Mathf.Deg2Rad);
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
}