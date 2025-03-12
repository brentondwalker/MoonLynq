
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using static IntersectionBase;
using static IntersectionNodeBase;

public class VehicleBehaviour : MonoBehaviour
{
    public GameObject vehicle;
    public IntersectionBase initialIntersection;
    public VehicleMobility mobility;
    public int initialIntersectionNode;

    private vehicleWayPoint wayPoint;
    private VehicleStatus vehicleStatus;

    private bool ini = false;

    public string statusDisplay;
    public float distanceToWayPoint;
    public string turnDisplay;
    public string trafficLightDisplay;


    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2f); 
        Start(); 
    }

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

        Vector3 pointMiddleStart = wayPoint.middleIntersection.node_Local[wayPoint.middleNodeStart].pointsPosition[3];
        Vector3 pointMiddleDest = wayPoint.middleIntersection.node_Local[wayPoint.middleNodeDest].pointsPosition[0];
        Vector3 currentPostion = vehicle.transform.position;

        switch (vehicleStatus)
        {
            case VehicleStatus.movingToNextIntersection:
                mobility.MoveToPoint(pointMiddleStart);
                break;

            case VehicleStatus.passthroughIntersection:
                mobility.MoveToPoint(pointMiddleDest);
                break;

            case VehicleStatus.stop:
                mobility.StopMovement();
                break;
        }

        if (Vector3.Distance(currentPostion, pointMiddleStart) < 1)
        {
            PassThroughIntersection(wayPoint);
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
    }

    public void PassThroughIntersection (vehicleWayPoint wayPoint)
    {
        IntersectionBase middleIntersection = wayPoint.middleIntersection;
        int middleNodeStart = wayPoint.middleNodeStart;
        int middleNodeDest = wayPoint.middleNodeDest;
        string trafficLightState = middleIntersection.intersection.GetTrafficLightState();

        trafficLightDisplay = trafficLightState.ToString();

        Dictionary<int, int> rightTurnMap = new Dictionary<int, int>()
        {
            { 0, 3 }, { 1, 0 }, { 2, 1 }, { 3, 2 }
        };

        Dictionary<int, int> leftTurnMap = new Dictionary<int, int>()
        {
            { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }
        };

        Dictionary<int, int> straightMap = new Dictionary<int, int>()
        {
            { 0, 2 }, { 1, 3 }, { 2, 0 }, { 3, 1 }
        };


        string turnType = "";

        if (middleNodeStart == 0 | middleNodeStart == 2) 
        {
            if (straightMap[middleNodeStart] == middleNodeDest)
            {
                turnType = "AC_Straight";
            }
            else if (rightTurnMap[middleNodeStart] == middleNodeDest)
            {
                turnType = "AC_RightTurn";
                turnDisplay = turnType;
                vehicleStatus = VehicleStatus.passthroughIntersection;
                return;
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
                turnDisplay = turnType;
                vehicleStatus = VehicleStatus.passthroughIntersection;
                return;
            }
            else if (leftTurnMap[middleNodeStart] == middleNodeDest)
            {
                turnType = "BD_LeftTurn";
            }
        }

        if (turnType == trafficLightState) { vehicleStatus = VehicleStatus.passthroughIntersection; }
        else { vehicleStatus = VehicleStatus.stop; }

        turnDisplay = turnType;
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
            int randomIndex = Random.Range(0, availableNodeIndices.Length);
            int selectedNodeId = availableNodeIndices[randomIndex];

            IntersectionNodeBase selectedNode = middleIntersection.node_Local[selectedNodeId];

            //Debug.Log($"Moving to node {selectedNodeId} in next intersection.");

            vehicleStatus = VehicleStatus.movingToNextIntersection;

            wayPoint.middleNodeDest = selectedNodeId;
            wayPoint.nextIntersection = selectedNode.node_Connection.parentIntersection;
            wayPoint.nextNode = selectedNode.node_Connection.nodeId;
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
    }
}
