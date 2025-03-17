using UnityEngine;

public class BlindSpot : MonoBehaviour
{
    public GameObject[] blindSpots = new GameObject[2];
    public Vector3[] position = new Vector3[2];
    public int nodeId;
    private IntersectionBase parentIntersection;

    private void Start()
    {
        for (int i = 0; i < blindSpots.Length; i++) { position[i] = blindSpots[i].transform.position; } 
    }

    public void setNodeId(int id) { nodeId = id; }
    public void setParentIntersection (IntersectionBase intersection) {  parentIntersection = intersection; }
    public IntersectionBase getParentIntersection() {  return parentIntersection; }

}
