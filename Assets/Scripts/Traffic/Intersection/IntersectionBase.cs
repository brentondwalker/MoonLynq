using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IntersectionNodeBase;

public class IntersectionBase : MonoBehaviour
{
    public class Intersection
    {
        public string intersectionName;

        public IntersectionNode nodeA;
        public IntersectionNode nodeB;
        public IntersectionNode nodeC;
        public IntersectionNode nodeD;

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
    }

    public enum TrafficLightState
    {
        AC_Straight,  
        AC_LeftTurn,  
        BD_Straight,  
        BD_LeftTurn   
    }

    private Intersection intersection;
    private TrafficLightState currentState;
    private Dictionary<TrafficLightState, float> stateDurations;


    public IntersectionNodeBase nodeA_Local;
    public IntersectionNodeBase nodeB_Local;
    public IntersectionNodeBase nodeC_Local;
    public IntersectionNodeBase nodeD_Local;


    public float AC_StraightDuration = 5f;
    public float BD_StraightDuration = 3f;
    public float AC_TurnDuration = 5f;
    public float BD_TurnDuration = 3f;

    public string interSectionName = "Intersection_0";

    void Start()
    {
        intersection = new Intersection();
        currentState = TrafficLightState.AC_Straight;

        intersection.intersectionName = interSectionName;

        stateDurations = new Dictionary<TrafficLightState, float>
        {
            { TrafficLightState.AC_Straight, AC_StraightDuration },  
            { TrafficLightState.AC_LeftTurn, AC_TurnDuration }, 
            { TrafficLightState.BD_Straight, BD_StraightDuration },  
            { TrafficLightState.BD_LeftTurn, BD_TurnDuration }  
        };

        StartCoroutine(SwitchTrafficLights());
    }

    private IEnumerator SwitchTrafficLights()
    {
        while (true)
        {
            intersection.SetTrafficLightState(currentState);
            Debug.Log($"{intersection.intersectionName} state: {currentState}, duration: {stateDurations[currentState]} seconds.");

            yield return new WaitForSeconds(stateDurations[currentState]); 

            currentState = (TrafficLightState)(((int)currentState + 1) % 4);
        }
    }

    
}
