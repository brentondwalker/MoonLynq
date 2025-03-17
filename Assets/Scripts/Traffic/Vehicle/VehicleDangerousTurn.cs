using UnityEngine;
using static VehicleBlindSpotDetector;

public class VehicleDangerousTurn : MonoBehaviour
{
    public VehicleBehaviour vehicleBehaviour;
    public VehicleBlindSpotDetector vehicleBlindSpotDetector;
    private bool isCount;
    public int rightTurnCount;
    public int dangerousTurnCount;
    private double dangerousTimeThreshold = 0.3;

    private void Start()
    {
        isCount = true;
        dangerousTurnCount = 0;
        rightTurnCount = 0;
    }

    private void Update()
    {
        BlindSpotInfo[] blindSpots = vehicleBlindSpotDetector.blindSpotInfos;
        if (vehicleBehaviour.dangerousTurn && isCount)
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
            isCount = false;
        }
        if (!vehicleBehaviour.dangerousTurn) { isCount = true; }
    }
}
