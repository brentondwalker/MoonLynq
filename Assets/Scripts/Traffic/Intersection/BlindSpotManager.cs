using UnityEngine;

public class BlindSpotManager : MonoBehaviour
{
    public BlindSpot[] blindSpotsNode;
    private void Start()
    {
        for (int i = 0; i < blindSpotsNode.Length; i++) { blindSpotsNode[i].setNodeId(i); }
    }
}
