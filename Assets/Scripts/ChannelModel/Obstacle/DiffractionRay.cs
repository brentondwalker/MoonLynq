using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class DiffractionRay : MonoBehaviour
{
    public GameObject objectA;
    private GameObject objectB;
    public string targetTag = "Obstacle";
    public LineRenderer lineRendererA;
    public LineRenderer lineRendererC;
    public bool collision = false;

    public string hitStatus = string.Empty;

    //public RaycastHit[] hitReverse;

    public UeInfo UeInfo;



    void Start()
    {
        objectB = UeInfo.TargetEnb.enbObject;
        LineInitialisation(lineRendererA);
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
        FindCorner();
    }


    void UpdateLinePosition(LineRenderer line, Vector3 direction)
    {
        Vector3 start = objectA.transform.position;
        Vector3 end = start + direction;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void LineInitialisation(LineRenderer line)
    {
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
    }

    void FindCorner()
    {
        Vector3 directionF = objectB.transform.position - objectA.transform.position;
        Vector3 newDirectionF = directionF;
        
        Ray rayF = new Ray(objectA.transform.position, directionF);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayF, out hitInfo, directionF.magnitude))
        {
            Collider obstacleCollider = hitInfo.collider;
            newDirectionF = ScanCorner(3, 30, directionF, obstacleCollider);
            newDirectionF = ScanCorner(0.3f, 3, newDirectionF, obstacleCollider);
            newDirectionF = ScanCorner(0.03f, 0.3f, newDirectionF, obstacleCollider);
            UpdateLinePosition(lineRendererA, newDirectionF);
            lineRendererA.enabled = true;
        }
        else
        {
            hitStatus = "No initial hit";
            lineRendererA.enabled = false;
        }



    }

    Vector3 ScanCorner(float angleStep, float angleMax, Vector3 directionF, Collider obstacleCollider)
    {
        RaycastHit hitInfo;
        Vector3 finalDirection = Vector3.zero;
        Vector3 lastDirection = directionF;
        for (float angle = angleStep; angle <= angleMax; angle += angleStep)
        {
            for (int deg = 0; deg < 360; deg = deg + 10)
            {
                Vector3 up = Vector3.Cross(directionF, Vector3.right).normalized;
                Vector3 right = Vector3.Cross(directionF, Vector3.up).normalized;
                Vector3 deviation = (up * Mathf.Sin(deg * Mathf.Deg2Rad) + right * Mathf.Cos(deg * Mathf.Deg2Rad)).normalized;
                Vector3 rotatedDir = Quaternion.AngleAxis(angle, deviation) * directionF;

                Ray offsetRay = new Ray(objectA.transform.position, rotatedDir);

                if (Physics.Raycast(offsetRay, out hitInfo, rotatedDir.magnitude))
                {
                    if (hitInfo.collider == obstacleCollider)
                    {
                        hitStatus = "No Edge found";
                    }
                    else
                    {
                        hitStatus = "Different obstacle found";
                    }
                }
                else
                {
                    hitStatus = "Edge found";
                    finalDirection = rotatedDir;
                    return lastDirection;
                }
                lastDirection = rotatedDir;
            }
        }
        return lastDirection;
    }

}
