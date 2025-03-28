using UnityEngine;
using static VehicleBlindSpotDetector;

public class VehicleDangerousTurn : MonoBehaviour
{
    public VehicleBehaviour vehicleBehaviour;
    public VehicleBlindSpotDetector vehicleBlindSpotDetector;
    private bool isDangerousCount;
    private bool isPassCount;
    public int rightTurnCount;
    public int dangerousTurnCount;
    public int passCount;
    private double dangerousTimeThreshold = 0.3;

    private void Start()
    {
        isDangerousCount = true;
        isPassCount = true;
        dangerousTurnCount = 0;
        rightTurnCount = 0;
        passCount = 0;
    }

    private void Update()
    {
        BlindSpotInfo[] blindSpots = vehicleBlindSpotDetector.blindSpotInfos;
        if (vehicleBehaviour.dangerousTurn && isDangerousCount)
        {
            if (vehicleBehaviour.dangerousIntersection == blindSpots[0].intersection)
            { rightTurnCount++; GlobalStatistikDangerousTurn.RightTurnCount(); }
            for (int i = 0; i < blindSpots.Length; i++)
            {
                if (vehicleBehaviour.dangerousIntersection == blindSpots[i].intersection
                    && vehicleBehaviour.dangerousTurnNode == blindSpots[i].nodeId)
                {
                    if (blindSpots[i].TimeSinceLastVisibleTime > dangerousTimeThreshold) 
                    { 
                        dangerousTurnCount++;
                        GlobalStatistikDangerousTurn.DangerousTurnCount();
                        break;
                    }
                }
            }
            isDangerousCount = false;
        }
        if (!vehicleBehaviour.dangerousTurn) { isDangerousCount = true; }

        if (vehicleBehaviour.passingThroughIntersection && isPassCount)
        {
            if (vehicleBehaviour.currentIntersection == blindSpots[0].intersection)
            { passCount++; GlobalStatistikDangerousTurn.IntersectionPassCount(); }
            isPassCount = false;
        }
        if (!vehicleBehaviour.passingThroughIntersection) { isPassCount = true; }
    }
}
