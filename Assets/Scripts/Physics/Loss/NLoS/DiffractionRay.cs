using System;
using UnityEngine;

public class DiffractionRay : MonoBehaviour
{
    public string targetTag = "Obstacle";
    public string floorTag = "Floor";
    public LineRenderer lineRendererA;
    public LineRenderer lineRendererB;
    public LineRenderer lineRendererC;
    public bool collision = false;

    public string hitStatusA = string.Empty;
    public string hitStatusB = string.Empty;
    public string hitStatusC = string.Empty;

    //public RaycastHit[] hitReverse;

    public KnifeEdgeObstacle knifeEdge;

    private float largestStructureSize = 80;
    private float edgeTolerance = 2;

    public double diffractionLoss = 0;
    public double pathLoss;
    public double totalLoss;


    private float scanDistanceBuffer;

    void Start()
    {
        LineInitialisation(lineRendererA);
        LineInitialisation(lineRendererB);
        LineInitialisation(lineRendererC);
    }

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
        
    }

    public double ComputeNlosDiffraction(Vector3 start, Vector3 dest, float frequency)
    {
        diffractionLoss = double.NaN;
        pathLoss = double.NaN;
        totalLoss = 0;
        double distance = double.MaxValue;

        Vector3 RayA = Vector3.zero;
        Vector3 RayB = Vector3.zero;
        Vector3 RayC = Vector3.zero;
        UpdateLinePosition(lineRendererA, Vector3.zero, Vector3.zero);
        UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
        UpdateLinePosition(lineRendererC, Vector3.zero, Vector3.zero);


        if (!IsLoS(start, dest))
        {
            scanDistanceBuffer = 0;
            RayA = FindFirstCorner(start, dest);
            RayC = RayToDest(RayA, start, dest);


            if (RayA != Vector3.zero && RayC == Vector3.zero)
            {
                scanDistanceBuffer = 0;
                RayB = FindSecondCorner(RayA, start, dest);
                double diffractionLossA = knifeEdge.ComputeSingleKnifeEdge(frequency, RayA, RayB);
                if (RayB != Vector3.zero)
                {
                    RayC = RayToDest(RayA + RayB, start, dest);
                    if (RayC != Vector3.zero)
                    {
                        double diffractionLossB = knifeEdge.ComputeSingleKnifeEdge(frequency, RayB, RayC);
                        diffractionLoss = knifeEdge.ComputeDoubleKnifeEdgeLc(diffractionLossA, diffractionLossB, RayA, RayB, RayC, start, dest);
                        distance = RayA.magnitude + RayB.magnitude + RayC.magnitude;
                        pathLoss = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
                    }
                    else diffractionLoss = double.NaN;
                }
                else diffractionLoss = double.NaN;
            }
            else if (RayA != Vector3.zero && RayC != Vector3.zero)
            {
                diffractionLoss = knifeEdge.ComputeSingleKnifeEdge(frequency, RayA, RayC);
                distance = RayA.magnitude + RayC.magnitude;
                pathLoss = 20 * Math.Log10(distance) + 20 * Math.Log10(frequency) - 147.55;
            }
            hitStatusB = "Not activated";
        }
        else hitStatusA = "No initial hit";
        totalLoss = pathLoss + diffractionLoss;
        return totalLoss;
    }

    void UpdateLinePosition(LineRenderer line,Vector3 start, Vector3 direction)
    {
        if (line == null) return;
        Vector3 end = start + direction;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void LineInitialisation(LineRenderer line)
    {
        if (line == null) return;   
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.positionCount = 2;
    }

    Vector3 FindFirstCorner(Vector3 start, Vector3 dest)
    {
        Vector3 directionF = dest - start;
        Vector3 newDirectionF = directionF;
        
        Ray rayF = new Ray(start, directionF);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayF, out hitInfo, directionF.magnitude))
        {
            if (hitInfo.collider.CompareTag(targetTag))
            {
                newDirectionF = ScanCorner(start, 1, 30, directionF, hitInfo, false, out hitStatusA);
                if (newDirectionF != Vector3.zero)
                {
                    newDirectionF = ScanCorner(start, 0.2f, 2.2f, newDirectionF, hitInfo, false, out hitStatusA);
                    newDirectionF = ScanCorner(start, 0.02f, 0.22f, newDirectionF, hitInfo, true, out hitStatusA);

                }
                UpdateLinePosition(lineRendererA, start, newDirectionF);
                return newDirectionF;
            }
            else
            {
                hitStatusA = "No initial hit, not target Tag";
                UpdateLinePosition(lineRendererA, Vector3.zero, Vector3.zero);
                return Vector3.zero;
            }
        }
        else
        {
            hitStatusA = "No initial hit";
            UpdateLinePosition(lineRendererA, Vector3.zero, Vector3.zero);
            return Vector3.zero;
        }
    }

    Vector3 FindSecondCorner(Vector3 direction, Vector3 start, Vector3 dest)
    {
        if (direction == Vector3.zero)
        {
            UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
            return Vector3.zero;
        }
        Vector3 rayStart = start + direction + direction.normalized * edgeTolerance/2;
        //Vector3 start = start + direction;
        Vector3 directionF = dest - rayStart;

        Vector3 newDirectionF = directionF;

        Ray rayF = new Ray(rayStart, directionF);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayF, out hitInfo, directionF.magnitude))
        {
            if (hitInfo.collider.CompareTag(targetTag))
            {
                newDirectionF = ScanCorner(rayStart, 1, 30, directionF, hitInfo, false, out hitStatusB);
                if (newDirectionF != Vector3.zero)
                {
                    newDirectionF = ScanCorner(rayStart, 0.2f, 2.2f, newDirectionF, hitInfo, false, out hitStatusB);
                    newDirectionF = ScanCorner(rayStart, 0.02f, 0.22f, newDirectionF, hitInfo, true, out hitStatusB);
                }
                UpdateLinePosition(lineRendererB, rayStart, newDirectionF);
                return newDirectionF;
            }
            else
            {
                hitStatusB = "No initial hit, not target Tag";
                UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
                return Vector3.zero;
            }
        }
        else
        {
            hitStatusB = "No initial hit";
            UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
            return Vector3.zero;
        }
    }

    Vector3 RayToDest (Vector3 direction, Vector3 start, Vector3 dest)
    {
        
        if (direction == Vector3.zero)
        {
            UpdateLinePosition(lineRendererC, Vector3.zero, Vector3.zero);
            hitStatusC = "No diffraction path found";
            return Vector3.zero;
        }
        
        for (float tol = 0; tol <= edgeTolerance; tol = tol + 0.5f)
        {
            Vector3 rayStart = start + direction + direction.normalized * tol;
            Vector3 rayDest = dest;
            RaycastHit hitInfo;
            Vector3 directionToDest = rayDest - rayStart;
            Ray toDest = new Ray(rayStart, directionToDest);

            if (Physics.Raycast(toDest, out hitInfo, directionToDest.magnitude))
            {
                if (!hitInfo.collider.CompareTag(targetTag))
                {
                    UpdateLinePosition(lineRendererC, rayStart, directionToDest);
                    hitStatusC = "Direct path found";
                    return directionToDest;
                }
                else
                {
                    UpdateLinePosition(lineRendererC, rayStart, Vector3.zero);
                    hitStatusC = "No direct path found";
                }
            }
            else
            {
                UpdateLinePosition(lineRendererC, rayStart, directionToDest);
                hitStatusC = "Direct path found";
                return directionToDest;
            }
        }
        hitStatusC = "No direct path found";
        return Vector3.zero;
    }

    Vector3 ScanCorner(Vector3 start, float angleStep, float angleMax, Vector3 directionF, RaycastHit hitInfo, bool returnFinalValue, out string statusString)
    {
        Collider obstacleCollider = hitInfo.collider;
        float rayRange = Vector3.Distance(start, hitInfo.point) + largestStructureSize;
        RaycastHit hitInfoNew;
        Vector3 finalDirection = Vector3.zero;
        Vector3 lastDirection = directionF;
        float lastDistance = 0;
        for (float angle = angleStep; angle <= angleMax; angle += angleStep)
        {
            for (int deg = 0; deg < 360; deg = deg + 10)
            {
                Vector3 up = Vector3.Cross(directionF, Vector3.right).normalized;
                Vector3 right = Vector3.Cross(directionF, Vector3.up).normalized;
                Vector3 deviation = (up * Mathf.Sin(deg * Mathf.Deg2Rad) + right * Mathf.Cos(deg * Mathf.Deg2Rad)).normalized;
                Vector3 rotatedDir = Quaternion.AngleAxis(angle, deviation) * directionF;

                Ray offsetRay = new Ray(start, rotatedDir);

                if (Physics.Raycast(offsetRay, out hitInfoNew, rayRange))
                {
                    if (hitInfoNew.collider.CompareTag(targetTag))
                    {
                        if (hitInfoNew.collider == obstacleCollider)
                        {
                            statusString = "No Edge found";
                        }
                        else
                        {
                            statusString = "Different obstacle found";
                        }
                        lastDistance = Vector3.Distance(start, hitInfoNew.point);
                        scanDistanceBuffer = Math.Max(scanDistanceBuffer, lastDistance);
                    }
                    else
                    {
                        if (hitInfoNew.collider.CompareTag(floorTag))
                        {
                            statusString = "Floor found";
                            lastDistance = scanDistanceBuffer;
                        }
                        else
                        {
                            statusString = "Edge found";
                            finalDirection = rotatedDir;
                            if (returnFinalValue)
                            {
                                finalDirection = finalDirection.normalized * lastDistance;
                                return finalDirection;
                            }
                            return lastDirection;
                        }
                    }
                }
                else
                {
                    statusString = "Edge found";
                    finalDirection = rotatedDir;
                    if (returnFinalValue)
                    {
                        finalDirection = finalDirection.normalized * lastDistance;
                        return finalDirection;
                    }
                    return lastDirection;
                }
                lastDirection = rotatedDir;
                
            }
        }
        statusString = "No Edge found";
        return Vector3.zero;
    }

    bool IsLoS (Vector3 start, Vector3 dest)
    {
        Ray losCheck = new Ray(start, dest - start);
        float rayRange = Vector3.Distance(start, dest);
        RaycastHit hitInfo;
        if (Physics.Raycast(losCheck, out hitInfo, rayRange))
        {
            if (hitInfo.collider.CompareTag(targetTag)) return false;
            else return true;
        }
        else return true;
    }
}
