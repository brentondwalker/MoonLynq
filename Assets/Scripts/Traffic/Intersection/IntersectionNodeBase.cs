using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Unity;
using static IntersectionBase;

public class IntersectionNodeBase : MonoBehaviour
{

    public IntersectionNodeBase node_Connection;

    public GameObject[] points = new GameObject[6];
    public LineRenderer[] node_Lines = new LineRenderer[3];

    private bool ini = false;
    public IntersectionBase parentIntersection;

    public int nodeId;
    public Vector3[] pointsPosition = new Vector3[6];

    void Start()
    {
        for (int i = 0; i < points.Length; i++) { pointsPosition[i] = points[i].transform.position; }
    }

    void Update()
    {
        if (node_Connection != null && !ini)
        {
            NodeLineConnection(node_Lines, node_Connection);
            for (int i = 0; i < points.Length; i++) { pointsPosition[i] = points[i].transform.position; }
        }
        ini = true;
    }

    void LineInitialisation(LineRenderer line)
    {
        if (line == null) return;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.positionCount = 2;
    }

    void UpdateLinePosition(LineRenderer line, Vector3 start, Vector3 end)
    {
        if (line == null) return;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void NodeLineConnection(LineRenderer[] line, IntersectionNodeBase targetNode)
    {
        for (int i = 0; i < line.Length; i++)
        {
            LineInitialisation(line[i]);
            if (targetNode != null && targetNode.points[i + line.Length] != null) UpdateLinePosition(line[i], pointsPosition[i], targetNode.pointsPosition[i + line.Length]);
        }
    }
}
