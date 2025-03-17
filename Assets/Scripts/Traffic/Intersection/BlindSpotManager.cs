using UnityEngine;

public class BlindSpotManager : MonoBehaviour
{
    public BlindSpot[] blindSpotsNode;
    public IntersectionBase parentIntersection;
    private void Start()
    {
        for (int i = 0; i < blindSpotsNode.Length; i++) 
        {
            blindSpotsNode[i].setNodeId(i);
            blindSpotsNode[i].setParentIntersection(parentIntersection);
        }
    }
}
