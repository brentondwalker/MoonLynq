
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IntersectionBase;
using static IntersectionNodeBase;

public class VehicleBehaviour : MonoBehaviour
{
    public GameObject vehicle;
    public IntersectionBase initialIntersection;
    public VehicleMobility mobility;
    public VehicleCollisionAvoidance vehicleCollision;

    public int initialIntersectionNode;

    private vehicleWayPoint wayPoint;
    private VehicleStatus vehicleStatus;

    private bool ini = false;

    public string statusDisplay;
    public float distanceToWayPoint;
    public string turnDisplay;
    public string trafficLightDisplay;

    public bool dangerousTurn;
    public int dangerousTurnNode;
    public IntersectionBase dangerousIntersection;

    private Dictionary<int, int> rightTurnMap = new Dictionary<int, int>()
        {
            { 0, 3 }, { 1, 0 }, { 2, 1 }, { 3, 2 }
        };

    private Dictionary<int, int> leftTurnMap = new Dictionary<int, int>()
        {
            { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }
        };

    private Dictionary<int, int> straightMap = new Dictionary<int, int>()
        {
            { 0, 2 }, { 1, 3 }, { 2, 0 }, { 3, 1 }
        };


    private bool isStartCompleted = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        wayPoint = new vehicleWayPoint();
        wayPoint.middleIntersection = initialIntersection;
        wayPoint.middleNodeStart = initialIntersectionNode;
        wayPoint.lastIntersection = initialIntersection.node_Local[initialIntersectionNode].node_Connection.parentIntersection;
        wayPoint.lastNode = initialIntersection.node_Local[initialIntersectionNode].node_Connection.nodeId;

        isStartCompleted = true;
    }


    private bool reachedMiddleDest = false;
    private bool reachedMiddleStart = false;


    private float stopTimer = 0f; 
    public float maxStopTime = 30f; 


    void Update()
    {
        if (!isStartCompleted) return;
        if (!ini) 
        {
            wayPoint.middleIntersection = initialIntersection;
            wayPoint.middleNodeStart = initialIntersectionNode;
            wayPoint.lastIntersection = initialIntersection.node_Local[initialIntersectionNode].node_Connection.parentIntersection;
            wayPoint.lastNode = initialIntersection.node_Local[initialIntersectionNode].node_Connection.nodeId;
            MoveToIntersection(wayPoint);
            ini = true;
        }

        //Debug.Log("pointMiddleStart: " + initialIntersection.node_Local[wayPoint.middleNodeStart].pointsPosition[3]);
        int laneId = 0;
        dangerousTurnNode = -1;
        dangerousTurn = false;
        dangerousIntersection = null;

        if (wayPoint.turnType == "AC_LeftTurn" || wayPoint.turnType == "BD_LeftTurn") laneId = 0;
        if (wayPoint.turnType == "AC_Straight" || wayPoint.turnType == "BD_Straight") laneId = 1;
        if (wayPoint.turnType == "AC_RightTurn" || wayPoint.turnType == "BD_RightTurn") laneId = 2;
        Vector3 pointMiddleStart = wayPoint.middleIntersection.node_Local[wayPoint.middleNodeStart].pointsPosition[5 - laneId];
        Vector3 pointMiddleDest = wayPoint.middleIntersection.node_Local[wayPoint.middleNodeDest].pointsPosition[2 - laneId];
        Vector3 currentPostion = vehicle.transform.position;

        if (vehicleCollision.CheckVehicleCollision() && vehicleStatus != VehicleStatus.passthroughIntersection)
        {
            mobility.StopMovement();
            return;
        }

        switch (vehicleStatus)
        {
            case VehicleStatus.movingToNextIntersection:
                mobility.MoveToPoint(pointMiddleStart);
                stopTimer = 0f;
                break;

            case VehicleStatus.passthroughIntersection:
                mobility.MoveToPoint(pointMiddleDest);
                stopTimer = 0f;

                if (wayPoint.turnType == "AC_RightTurn" || wayPoint.turnType == "BD_RightTurn")
                {
                    dangerousTurn = true;
                    dangerousTurnNode = wayPoint.middleNodeDest;
                    dangerousIntersection = wayPoint.middleIntersection;
                }

                break;

            case VehicleStatus.stop:
                mobility.StopMovement();
                stopTimer += Time.deltaTime; 

                if (stopTimer >= maxStopTime)
                {
                    vehicleStatus = VehicleStatus.movingToNextIntersection; 
                    stopTimer = 0f; 
                }
                break;
        }

        if (!reachedMiddleStart && Vector3.Distance(currentPostion, pointMiddleStart) < 1)
        {
            PassThroughIntersection(wayPoint);
            reachedMiddleStart = true;
        }

        if (reachedMiddleStart && Vector3.Distance(currentPostion, pointMiddleDest) > 1.5f)
        {
            reachedMiddleStart = false;
        }

        if (!reachedMiddleDest && Vector3.Distance(currentPostion, pointMiddleDest) < 1)
        {
            wayPoint.lastIntersection = wayPoint.middleIntersection;
            wayPoint.lastNode = wayPoint.middleNodeDest;
            wayPoint.middleIntersection = wayPoint.nextIntersection;
            wayPoint.middleNodeStart = wayPoint.nextNode;
            MoveToIntersection(wayPoint);
            reachedMiddleDest = true; 
        }

        if (reachedMiddleDest && Vector3.Distance(currentPostion, pointMiddleDest) > 1.5f)
        {
            reachedMiddleDest = false;
        }



        statusDisplay = vehicleStatus.ToString();
        distanceToWayPoint = Vector3.Distance(currentPostion, pointMiddleStart);
        turnDisplay = wayPoint.turnType;
    }

    public void PassThroughIntersection (vehicleWayPoint wayPoint)
    {
        IntersectionBase middleIntersection = wayPoint.middleIntersection;
        string trafficLightState = middleIntersection.intersection.GetTrafficLightState();

        trafficLightDisplay = trafficLightState.ToString();


        if (wayPoint.turnType == "AC_RightTurn" || wayPoint.turnType == "BD_RightTurn")
        {
            vehicleStatus = VehicleStatus.passthroughIntersection;
            return;
        }
        if (wayPoint.turnType == trafficLightState) { vehicleStatus = VehicleStatus.passthroughIntersection; }
        else { vehicleStatus = VehicleStatus.stop; }

    }


    public void MoveToIntersection (vehicleWayPoint wayPoint)
    {
        IntersectionBase lastIntersection = wayPoint.lastIntersection;
        int lastNode = wayPoint.lastNode;
        IntersectionBase middleIntersection = wayPoint.middleIntersection;

        int middleNodeId = lastIntersection.node_Local[lastNode].node_Connection.nodeId;
        IntersectionNodeBase middleNode = middleIntersection.node_Local[middleNodeId];

        int[] availableNodeIndices = Enumerable.Range(0, middleIntersection.node_Local.Length)
                                           .Where(i => i != middleNodeId && middleIntersection.node_Local[i].node_Connection != null)
                                           .ToArray();

        if (availableNodeIndices.Length > 0)
        {
            Random.InitState(System.DateTime.Now.Millisecond + GetInstanceID());
            int randomIndex = Random.Range(0, availableNodeIndices.Length);
            int selectedNodeId = availableNodeIndices[randomIndex];

            IntersectionNodeBase selectedNode = middleIntersection.node_Local[selectedNodeId];

            //Debug.Log($"Moving to node {selectedNodeId} in next intersection.");

            vehicleStatus = VehicleStatus.movingToNextIntersection;

            wayPoint.middleNodeDest = selectedNodeId;
            wayPoint.nextIntersection = selectedNode.node_Connection.parentIntersection;
            wayPoint.nextNode = selectedNode.node_Connection.nodeId;


            string turnType = "";
            int middleNodeStart = wayPoint.middleNodeStart;
            int middleNodeDest = wayPoint.middleNodeDest;

            if (middleNodeStart == 0 || middleNodeStart == 2)
            {
                if (straightMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "AC_Straight";
                }
                else if (rightTurnMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "AC_RightTurn";
                }
                else if (leftTurnMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "AC_LeftTurn";
                }
            }
            else
            {
                if (straightMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "BD_Straight";
                }
                else if (rightTurnMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "BD_RightTurn";
                }
                else if (leftTurnMap[middleNodeStart] == middleNodeDest)
                {
                    turnType = "BD_LeftTurn";
                }
            }
            wayPoint.turnType = turnType;
            
        }
    }

    enum VehicleStatus
    {
        movingToNextIntersection,
        passthroughIntersection,
        stop
    }

    public class vehicleWayPoint
    {
        public IntersectionBase lastIntersection;
        public int lastNode;
        public IntersectionBase middleIntersection;
        public int middleNodeStart;
        public int middleNodeDest;
        public IntersectionBase nextIntersection;
        public int nextNode;

        public string turnType;
    }
}
