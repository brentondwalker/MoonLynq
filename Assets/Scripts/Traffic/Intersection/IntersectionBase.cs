using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IntersectionNodeBase;

public class IntersectionBase : MonoBehaviour
{
    public class Intersection
    {
        public string intersectionName;

        //public IntersectionNode[] node = new IntersectionNode[4];

        public Dictionary<TrafficLightState, bool> trafficLights = new Dictionary<TrafficLightState, bool>();

        public Intersection()
        {
            foreach (TrafficLightState state in System.Enum.GetValues(typeof(TrafficLightState)))
            {
                trafficLights[state] = false;
            }
        }

        public void SetTrafficLightState(TrafficLightState activeState)
        {
            List<TrafficLightState> keys = new List<TrafficLightState>(trafficLights.Keys);

            foreach (TrafficLightState state in keys)
            {
                trafficLights[state] = (state == activeState);
            }
        }

        public string GetTrafficLightState()
        {
            foreach (var kvp in trafficLights)
            {
                if (kvp.Value) 
                {
                    return kvp.Key.ToString();
                }
            }
            return "None";
        }
    }

    public enum TrafficLightState
    {
        AC_Straight,  
        AC_LeftTurn,  
        BD_Straight,  
        BD_LeftTurn   
    }

    public Intersection intersection;
    private TrafficLightState currentState;
    private Dictionary<TrafficLightState, float> stateDurations;


    public IntersectionNodeBase[] node_Local = new IntersectionNodeBase[4];


    public float AC_StraightDuration = 5f;
    public float BD_StraightDuration = 3f;
    public float AC_TurnDuration = 5f;
    public float BD_TurnDuration = 3f;

    public string interSectionName = "Intersection_0";

    void Start()
    {
        intersection = new Intersection();
        currentState = TrafficLightState.AC_Straight;

        for (int i = 0; i < node_Local.Length; i++)
        {
            node_Local[i].nodeId = i;
        }

        intersection.intersectionName = interSectionName;

        if (node_Local[0].node_Connection == null || node_Local[2].node_Connection == null) AC_StraightDuration = 0;
        if (node_Local[1].node_Connection == null || node_Local[3].node_Connection == null) BD_StraightDuration = 0;
        if ((node_Local[0].node_Connection == null && node_Local[1].node_Connection == null) || (node_Local[2].node_Connection == null && node_Local[3].node_Connection == null)) BD_TurnDuration = 0;
        if ((node_Local[1].node_Connection == null && node_Local[2].node_Connection == null) || (node_Local[0].node_Connection == null && node_Local[3].node_Connection == null)) AC_TurnDuration = 0;

        stateDurations = new Dictionary<TrafficLightState, float>
        {
            { TrafficLightState.AC_Straight, AC_StraightDuration },  
            { TrafficLightState.AC_LeftTurn, AC_TurnDuration }, 
            { TrafficLightState.BD_Straight, BD_StraightDuration },  
            { TrafficLightState.BD_LeftTurn, BD_TurnDuration }  
        };

        StartCoroutine(SwitchTrafficLights());
    }

    private void Update()
    {
        for (int i = 0; i < node_Local.Length; i++)
        {
            node_Local[i].nodeId = i;
        }
    }

    private IEnumerator SwitchTrafficLights()
    {
        while (true)
        {
            intersection.SetTrafficLightState(currentState);
            //Debug.Log($"{intersection.intersectionName} state: {currentState}, duration: {stateDurations[currentState]} seconds.");

            yield return new WaitForSeconds(stateDurations[currentState]); 

            currentState = (TrafficLightState)(((int)currentState + 1) % 4);
        }
    }

    
}
