using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class DiffractionRay : MonoBehaviour
{
    public GameObject objectA;
    private GameObject objectB;
    public string targetTag = "Obstacle";
    public LineRenderer lineRendererA;
    public LineRenderer lineRendererB;
    public LineRenderer lineRendererC;
    public bool collision = false;

    public string hitStatusA = string.Empty;
    public string hitStatusB = string.Empty;
    public string hitStatusC = string.Empty;

    //public RaycastHit[] hitReverse;

    public UeInfo UeInfo;

    private float largestStructureSize = 50;
    private float edgeTolerance = 2;



    void Start()
    {
        objectB = UeInfo.TargetEnb.enbObject;
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
        Vector3 RayA = Vector3.zero;
        Vector3 RayB = Vector3.zero;
        Vector3 RayC = Vector3.zero;
        UpdateLinePosition(lineRendererA, Vector3.zero, Vector3.zero);
        UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
        UpdateLinePosition(lineRendererC, Vector3.zero, Vector3.zero);
        RayA = FindFirstCorner();
        RayC = rayToDest(RayA);
        if (RayA != Vector3.zero && RayC == Vector3.zero)
        {
            RayB = FindSecondCorner(RayA);
            if (RayB != Vector3.zero) RayC = rayToDest(RayA + RayB);
        }
        else hitStatusB = "Not activated";
    }


    void UpdateLinePosition(LineRenderer line,Vector3 start, Vector3 direction)
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

    Vector3 FindFirstCorner()
    {
        Vector3 directionF = objectB.transform.position - objectA.transform.position;
        Vector3 newDirectionF = directionF;
        
        Ray rayF = new Ray(objectA.transform.position, directionF);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayF, out hitInfo, directionF.magnitude))
        {
            newDirectionF = ScanCorner(objectA.transform.position, 3, 30, directionF, hitInfo, false, out hitStatusA);
            if (newDirectionF != Vector3.zero) 
            {
                newDirectionF = ScanCorner(objectA.transform.position, 0.3f, 3, newDirectionF, hitInfo, false, out hitStatusA);
                newDirectionF = ScanCorner(objectA.transform.position, 0.03f, 0.3f, newDirectionF, hitInfo, true, out hitStatusA);
            }
            UpdateLinePosition(lineRendererA,objectA.transform.position, newDirectionF);
            lineRendererA.enabled = true;
            return newDirectionF;
        }
        else
        {
            hitStatusA = "No initial hit";
            lineRendererA.enabled = false;
            return Vector3.zero;
        }
    }

    Vector3 FindSecondCorner(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            UpdateLinePosition(lineRendererB, Vector3.zero, Vector3.zero);
            return Vector3.zero;
        }
        Vector3 start = objectA.transform.position + direction + direction.normalized * edgeTolerance/2;
        //Vector3 start = objectA.transform.position + direction;
        Vector3 directionF = objectB.transform.position - start;

        Vector3 newDirectionF = directionF;

        Ray rayF = new Ray(start, directionF);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayF, out hitInfo, directionF.magnitude))
        {
            newDirectionF = ScanCorner(start, 3, 30, directionF, hitInfo, false, out hitStatusB);
            if (newDirectionF != Vector3.zero)
            {
                newDirectionF = ScanCorner(start, 0.3f, 3, newDirectionF, hitInfo, false, out hitStatusB);
                newDirectionF = ScanCorner(start, 0.03f, 0.3f, newDirectionF, hitInfo, true, out hitStatusB);
            }
            UpdateLinePosition(lineRendererB, start, newDirectionF);
            lineRendererB.enabled = true;
            return newDirectionF;
        }
        else
        {
            hitStatusB = "No initial hit";
            lineRendererB.enabled = false;
            return Vector3.zero;
        }
    }

    Vector3 rayToDest (Vector3 direction)
    {
        
        if (direction == Vector3.zero)
        {
            UpdateLinePosition(lineRendererC, Vector3.zero, Vector3.zero);
            hitStatusC = "No diffraction path found";
            return Vector3.zero;
        }
        
        for (float tol = 0; tol <= edgeTolerance; tol = tol + 0.5f)
        {
            Vector3 start = objectA.transform.position + direction + direction.normalized * tol;
            Vector3 dest = objectB.transform.position;
            RaycastHit hitInfo;
            Vector3 directionToDest = dest - start;
            Ray toDest = new Ray(start, directionToDest);

            if (Physics.Raycast(toDest, out hitInfo, directionToDest.magnitude))
            {
                UpdateLinePosition(lineRendererC, start, Vector3.zero);
                hitStatusC = "No direct path found";
            }
            else
            {
                UpdateLinePosition(lineRendererC, start, directionToDest);
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
                    if (hitInfoNew.collider == obstacleCollider)
                    {
                        statusString = "No Edge found";
                    }
                    else
                    {
                        statusString = "Different obstacle found";
                    }
                    lastDistance = Vector3.Distance(start, hitInfoNew.point);
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

}
