using System;
using Unity.VisualScripting;
using UnityEngine;

public class ReflectionRay : MonoBehaviour
{
    public double[] absorptionLoss;
    public double[] pathLoss;
    public double[] totalLoss;
    
    public GameObject objectA;
    private GameObject objectB;
    public string targetTag = "Obstacle";
    public LineRenderer[] lineRendererA;
    public LineRenderer[] lineRendererB;
    public int lineCount = 6;
    public bool collision = false;

    public string hitStatusA = string.Empty;
    public string hitStatusB = string.Empty;


    public UeInfo UeInfo;
    public Reflectance reflectance;
    public MaterialProperties mediumMaterial;

    private Vector3 start;
    private Vector3 dest;
    private Vector3 destH;

    private float disTolerance = 20;
    private float finalDisTolereance = 10;


    void Start()
    {

        objectB = UeInfo.TargetEnb.enbObject;
        for (int i = 0; i < lineCount; i++)
        {
            LineInitialisation(lineRendererA[i]);
            LineInitialisation(lineRendererB[i]);
        }
    }

    // Update is called once per frame
    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 0.1f;


    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }

    void CustomUpdate()
    {
        start = objectA.transform.position;
        dest = objectB.transform.position;
        destH = Vector3.zero;
        destH.x = dest.x;
        destH.z = dest.z;


        reflectionRayGroup[] reflectionRayGroup = new reflectionRayGroup[lineCount];
        for (int i = 0;i < lineCount;i++) 
        {
            UpdateLinePosition(lineRendererA[i], Vector3.zero, Vector3.zero);
            UpdateLinePosition(lineRendererB[i], Vector3.zero, Vector3.zero);

            absorptionLoss[i] = 0;
            pathLoss[i] = 0;
            totalLoss[i] = 0;
        }

        int step = 360 / lineCount;

        for (int i = 0; i < lineCount; i++)
        {
            absorptionLoss[i] = double.NaN;
            pathLoss[i] = double.NaN;
            reflectionRayGroup[i] = CastReflectionRayH(step * (i - 1), step * i, 10);
            if (reflectionRayGroup[i].minDistance < disTolerance)
            {
                reflectionRayGroup[i] = CastReflectionRayH(reflectionRayGroup[i].closestDeg - 11, reflectionRayGroup[i].closestDeg + 11, 0.5f);
                float closestDegH = reflectionRayGroup[i].closestDeg;
                if (closestDegH  < step * i && closestDegH >= step * (i - 1))
                {
                    reflectionRayGroup[i] = CastReflectionRayV(closestDegH, 0, 90, 5);
                    if (reflectionRayGroup[i].minDistance < disTolerance)
                    {
                        reflectionRayGroup[i] = CastReflectionRayV(closestDegH, reflectionRayGroup[i].closestDeg - 5, reflectionRayGroup[i].closestDeg + 5, 0.5f);
                        if (reflectionRayGroup[i].minDistance < finalDisTolereance && reflectionRayGroup[i].incidenceHit.gameObject.GetComponent<MaterialProperties>() != null)
                        {
                            float distanceIncidence = Vector3.Distance(start, reflectionRayGroup[i].hitPoint);
                            float distanceReflection = Vector3.Distance(reflectionRayGroup[i].hitPoint, dest);

                            UpdateLinePosition(lineRendererA[i], start, reflectionRayGroup[i].incidenceDirection.normalized * distanceIncidence);
                            UpdateLinePosition(lineRendererB[i], reflectionRayGroup[i].hitPoint, reflectionRayGroup[i].reflectionDirection.normalized * distanceReflection);

                            MaterialProperties refractiveMaterial = reflectionRayGroup[i].incidenceHit.gameObject.GetComponent<MaterialProperties>();
                            float angle = Vector3.Angle(reflectionRayGroup[i].hitNormal, -reflectionRayGroup[i].incidenceDirection.normalized);
                            absorptionLoss[i] = reflectance.ComputeReflectance(mediumMaterial, refractiveMaterial, angle * Mathf.Deg2Rad);
                            absorptionLoss[i] = -PowerCalculator.linearToDb(absorptionLoss[i]);
                            double distance = distanceIncidence + distanceReflection;
                            pathLoss[i] = 20 * Math.Log10(distance) + 20 * Math.Log10(UeInfo.frequency) - 147.55;
                        }
                    }
                }
            }
            
            totalLoss[i] = pathLoss[i] + absorptionLoss[i];
        }
    }


    reflectionRayGroup CastReflectionRayH(float degStart, float degEnd, float degStep) 
    {
        reflectionRayGroup rayGroup = new reflectionRayGroup();
        Vector3 closestIncidence = Vector3.zero;
        Vector3 closestReflection = Vector3.zero;
        Vector3 closestHit = Vector3.zero;
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
        return rayGroup;
    }

    reflectionRayGroup CastReflectionRayV(float degH, float degVStart, float degVEnd, float degVStep)
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



    void UpdateLinePosition(LineRenderer line, Vector3 start, Vector3 direction)
    {
        Vector3 end = start + direction;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void LineInitialisation(LineRenderer line)
    {
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
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
