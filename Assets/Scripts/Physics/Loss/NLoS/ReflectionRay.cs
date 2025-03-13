using System;
using UnityEngine;
using static TransmissionParameterManager;

public class ReflectionRay : MonoBehaviour
{
    public double[] absorptionLoss;
    public double[] pathLoss;
    public double[] totalLoss;
    
    public string targetTag = "Obstacle";
    public LineRenderer[] lineRendererA;
    public LineRenderer[] lineRendererB;
    public int lineCount = 6;
    public bool collision = false;

    public string hitStatusA = string.Empty;
    public string hitStatusB = string.Empty;

    public Reflectance reflectance;
    public MaterialProperties mediumMaterial;

    private float disTolerance = 20;
    private float finalDisTolereance = 10;

    public bool useVerticalScan = true;

    public float lineWidth = 0.5f;

    private float scanDistanceBuffer;

    void Start()
    {
        for (int i = 0; i < lineCount; i++)
        {
            LineInitialisation(lineRendererA[i]);
            LineInitialisation(lineRendererB[i]);
        }
    }

    private void Update()
    {
        IdleDetection();
    }

    private float lastUpdateTime = -1f;

    void IdleDetection()
    {

        if (Time.time - lastUpdateTime > 0.5)
        {
            for (int i = 0; i < lineCount; i++)
            {
                if (lineRendererA.Length != 0 && lineRendererB.Length != 0)
                {
                    UpdateLinePosition(lineRendererA[i], Vector3.zero, Vector3.zero);
                    UpdateLinePosition(lineRendererB[i], Vector3.zero, Vector3.zero);
                }

            }

        }
        
    }

    public double[] ComputeNlosReflection(TransmissionParameter parameter)
    {

        lastUpdateTime = parameter.lastUpdateTime;


        Vector3 start = parameter.positionA;
        Vector3 dest = parameter.positionB;
        float frequency = parameter.frequency;
        
        
        Vector3 destH = Vector3.zero;
        destH.x = dest.x;
        destH.y = start.y;
        destH.z = dest.z;


        reflectionRayGroup[] reflectionRayGroup = new reflectionRayGroup[lineCount];
        for (int i = 0; i < lineCount; i++)
        {
            if (lineRendererA != null && lineRendererB != null && lineRendererA.Length >= lineCount && lineRendererB.Length >= lineCount)
            {
                UpdateLinePosition(lineRendererA[i], Vector3.zero, Vector3.zero);
                UpdateLinePosition(lineRendererB[i], Vector3.zero, Vector3.zero);
            }
            else
            {
                lineRendererA = new LineRenderer[lineCount];
                lineRendererB = new LineRenderer[lineCount];   
            }

            absorptionLoss[i] = 0;
            pathLoss[i] = 0;
            totalLoss[i] = 0;
        }

        int step = 360 / lineCount;

        for (int i = 0; i < lineCount; i++)
        {
            absorptionLoss[i] = double.NaN;
            pathLoss[i] = double.NaN;
            reflectionRayGroup[i] = CastReflectionRayH(step * (i - 1), step * i, 10, start, destH);
            if (reflectionRayGroup[i].minDistance < disTolerance)
            {
                reflectionRayGroup[i] = CastReflectionRayH(reflectionRayGroup[i].closestDeg - 11, reflectionRayGroup[i].closestDeg + 11, 0.5f, start, destH);
                float closestDegH = reflectionRayGroup[i].closestDeg;
                if (closestDegH < step * i && closestDegH >= step * (i - 1))
                {
                    if (useVerticalScan) reflectionRayGroup[i] = CastReflectionRayV(closestDegH, -90, 90, 5, start, dest, destH);
                    if (reflectionRayGroup[i].minDistance < disTolerance)
                    {
                        if (useVerticalScan) reflectionRayGroup[i] = CastReflectionRayV(closestDegH, reflectionRayGroup[i].closestDeg - 5, reflectionRayGroup[i].closestDeg + 5, 0.5f, start, dest, destH);
                        if (reflectionRayGroup[i].minDistance < finalDisTolereance && reflectionRayGroup[i].incidenceHit.gameObject.GetComponent<MaterialProperties>() != null)
                        {
                            //Debug.Log("Final reflection scan");

                            float distanceIncidence = Vector3.Distance(start, reflectionRayGroup[i].hitPoint);
                            float distanceReflection = Vector3.Distance(reflectionRayGroup[i].hitPoint, dest);

                            UpdateLinePosition(lineRendererA[i], start, reflectionRayGroup[i].incidenceDirection.normalized * distanceIncidence);
                            UpdateLinePosition(lineRendererB[i], reflectionRayGroup[i].hitPoint, reflectionRayGroup[i].reflectionDirection.normalized * distanceReflection);

                            MaterialProperties refractiveMaterial = reflectionRayGroup[i].incidenceHit.gameObject.GetComponent<MaterialProperties>();
                            float angle = Vector3.Angle(reflectionRayGroup[i].hitNormal, -reflectionRayGroup[i].incidenceDirection.normalized);
                            absorptionLoss[i] = reflectance.ComputeReflectance(mediumMaterial, refractiveMaterial, angle * Mathf.Deg2Rad);
                            absorptionLoss[i] = -PowerCalculator.linearToDb(absorptionLoss[i]);
                            double distance = distanceIncidence + distanceReflection;
                            pathLoss[i] = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
                        }
                    }
                }
            }

            totalLoss[i] = pathLoss[i] + absorptionLoss[i];
        }
        return totalLoss;
    }


    reflectionRayGroup CastReflectionRayH(float degStart, float degEnd, float degStep, Vector3 start, Vector3 destH) 
    {
        reflectionRayGroup rayGroup = new reflectionRayGroup();
        Vector3 closestIncidence = Vector3.zero;
        Vector3 closestReflection = Vector3.zero;
        Vector3 closestHit = Vector3.zero;
        Collider incidenceHit = null;
        float minDistance = float.MaxValue;
        float closestDeg = 0;
        Vector3 direction = Vector3.zero;

        hitStatusA = "No init hit";

        for (float deg = degStart; deg <= degEnd; deg = deg + degStep)
        {
            float range = Vector3.Distance(start, destH);
            direction = new Vector3(Mathf.Cos(deg * Mathf.Deg2Rad), 0, Mathf.Sin(deg * Mathf.Deg2Rad)) ;
            Ray ray = new Ray(start, direction * range);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range))
            {
                Vector3 hitPosition = hit.point;
                incidenceHit = hit.collider;
                float hitDistance = hit.distance;
                Vector3 normal = hit.normal; 
                Vector3 reflectDirection = Vector3.Reflect(direction, normal).normalized;

                Ray reflectionRay = new Ray(hitPosition, reflectDirection);

                float distance = DistancePointToLine(destH, reflectionRay.origin, reflectionRay.direction);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIncidence = direction;
                    closestReflection = reflectDirection;
                    closestHit = hitPosition;
                    closestDeg = deg;
                }

                hitStatusA = "Reflecion path found";
            }
            else
            {
            }
        }
        rayGroup.incidenceDirection = closestIncidence;
        rayGroup.reflectionDirection = closestReflection;
        rayGroup.hitPoint = closestHit;
        rayGroup.minDistance = minDistance;
        rayGroup.closestDeg = closestDeg;
        rayGroup.incidenceHit = incidenceHit;
        return rayGroup;
    }

    reflectionRayGroup CastReflectionRayV(float degH, float degVStart, float degVEnd, float degVStep, Vector3 start, Vector3 dest, Vector3 destH)
    {
        reflectionRayGroup rayGroup = new reflectionRayGroup();
        Vector3 closestIncidence = Vector3.zero;
        Vector3 closestReflection = Vector3.zero;
        Vector3 closestHit = Vector3.zero;
        Collider incidenceHit = null;
        float minDistance = float.MaxValue;
        float closestDeg = 0;
        Vector3 direction = Vector3.zero;

        hitStatusB = "No init hit";

        for (float degV = degVStart; degV <= degVEnd; degV = degV + degVStep)
        {
            float range = Vector3.Distance(start, destH);
            direction = new Vector3(Mathf.Cos(degH * Mathf.Deg2Rad), Mathf.Cos(degV * Mathf.Deg2Rad), Mathf.Sin(degH * Mathf.Deg2Rad));
            Ray ray = new Ray(start, direction * range);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range))
            {
                Vector3 hitPosition = hit.point;
                float hitDistance = hit.distance;
                Vector3 normal = hit.normal;
                Vector3 reflectDirection = Vector3.Reflect(direction, normal).normalized;
                incidenceHit = hit.collider;
                rayGroup.hitNormal = hit.normal;

                Ray reflectionRay = new Ray(hitPosition, reflectDirection * Vector3.Distance(hitPosition, dest));

                RaycastHit reflectionHit = new RaycastHit();

                if (Physics.Raycast(reflectionRay, out reflectionHit))
                {
                    continue;
                }

                float distance = DistancePointToLine(dest, reflectionRay.origin, reflectionRay.direction);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIncidence = direction;
                    closestReflection = reflectDirection;
                    closestHit = hitPosition;
                    closestDeg = degV;
                }

                hitStatusB = "Reflecion path found";
            }
            else
            {
            }
        }
        rayGroup.incidenceDirection = closestIncidence;
        rayGroup.reflectionDirection = closestReflection;
        rayGroup.hitPoint = closestHit;
        rayGroup.minDistance = minDistance;
        rayGroup.closestDeg = closestDeg;
        rayGroup.incidenceHit = incidenceHit;
        return rayGroup;
    }



    void UpdateLinePosition(LineRenderer line, Vector3 start, Vector3 direction)
    {
        if (line == null) return;
        Vector3 end = start + direction;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void LineInitialisation(LineRenderer line)
    {
        if (line == null) return;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
    }

    float DistancePointToLine(Vector3 point, Vector3 rayOrigin, Vector3 rayDirection)
    {
        Vector3 pointToOrigin = point - rayOrigin;
        Vector3 projected = Vector3.Project(pointToOrigin, rayDirection);

        if (Vector3.Dot(projected, rayDirection) < 0)
        {
            return pointToOrigin.magnitude;
        }

        return (pointToOrigin - projected).magnitude;
    }

    class reflectionRayGroup
    {
        public Vector3 incidenceDirection = Vector3.zero;
        public Vector3 reflectionDirection = Vector3.zero;
        public Vector3 hitPoint = Vector3.zero;
        public float minDistance = float.MaxValue;
        public float closestDeg;
        public Collider incidenceHit;
        public Vector3 hitNormal;
    }

}
