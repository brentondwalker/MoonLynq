using UnityEngine;

public class VehicleBlindSpotDetector : MonoBehaviour
{
    public UeBase ueBase;

    public string targetTag = "Obstacle";
    private float detectionRange = 80;

    private Vector3 vehiclePosition;
    private BlindSpot[] allBlindSpots;
    public BlindSpotInfo[] blindSpotInfos = new BlindSpotInfo[8];

    public string blindSpotInfoDisplay = "";



    void Start()
    {
        vehiclePosition = ueBase.ueObject.transform.position;
        allBlindSpots = FindObjectsByType<BlindSpot>(FindObjectsSortMode.InstanceID);

        for (int i = 0; i < blindSpotInfos.Length; i++)
        {
            blindSpotInfos[i] = new BlindSpotInfo
            {
                nodeId = -1,
                pointId = -1,
                isVisible = false,
                lastVisibleTime = -1
            };
        }
    }

    void Update()
    {
        vehiclePosition = ueBase.ueObject.transform.position;

        int index = 0; 
        foreach (var blindSpot in allBlindSpots)
        {
            for (int i = 0; i < blindSpot.position.Length; i++)
            {
                if (index >= 8) break; 

                Vector3 blindSpotPosition = blindSpot.position[i];
                float distance = Vector3.Distance(vehiclePosition, blindSpotPosition);

                if (distance <= detectionRange)
                {
                    bool isBlocked = IsBlockedByObstacle(vehiclePosition, blindSpotPosition);

                    blindSpotInfos[index].nodeId = blindSpot.nodeId;
                    blindSpotInfos[index].pointId = i;
                    blindSpotInfos[index].intersection = blindSpot.getParentIntersection();
                    blindSpotInfos[index].isVisible = !isBlocked;
                    if (!isBlocked)
                    {
                        blindSpotInfos[index].lastVisibleTime = Time.time;
                    }
                }
                else
                {
                    blindSpotInfos[index].nodeId = blindSpot.nodeId;
                    blindSpotInfos[index].pointId = i;
                    blindSpotInfos[index].intersection = blindSpot.getParentIntersection();
                    blindSpotInfos[index].isVisible = false;
                }

                index++;
            }


            blindSpotInfoDisplay = GetBlindSpotInfoDisplay();
        }


        for (; index < 8; index++)
        {
            blindSpotInfos[index].nodeId = -1;
            blindSpotInfos[index].pointId = -1;
            blindSpotInfos[index].isVisible = false;
            blindSpotInfos[index].lastVisibleTime = -1;
            blindSpotInfos[index].lastVisibleTimeViaCommunication = -1;
            blindSpotInfos[index].intersection = null;
        }
    }

    private bool IsBlockedByObstacle(Vector3 start, Vector3 end)
    {
        RaycastHit[] hits = Physics.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    public class BlindSpotInfo
    {
        public int nodeId;
        public int pointId;
        public bool isVisible;
        public double lastVisibleTime;
        public double lastVisibleTimeViaCommunication;
        public double TimeSinceLastVisibleTime;

        public IntersectionBase intersection;
    }

    public string GetBlindSpotInfoDisplay()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var info in blindSpotInfos)
        {
            sb.AppendLine($"NID: {info.nodeId}, PID: {info.pointId}, " +
                          $"VS: {info.isVisible}, LVT: {info.lastVisibleTime:F2}");
        }

        return sb.ToString();
    }
}