using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Unity;

public class IntersectionNodeBase : MonoBehaviour
{
    public class IntersectionNode
    {
        public Vector3[] points = new Vector3[6];
        public IntersectionNode nodeConnection;
    }

    public IntersectionNodeBase node_Connection;

    public IntersectionNode node;

    public GameObject[] points = new GameObject[6];
    public LineRenderer[] node_Lines = new LineRenderer[3];

    private bool ini = false;

    void Start()
    {
        node = new IntersectionNode();
        for (int i = 0; i < points.Length; i++) { node.points[i] = points[i].transform.position; }
    }

    void Update()
    {
        if (node_Connection != null && !ini) NodeLineConnection(node_Lines, node, node_Connection.node);
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

    void NodeLineConnection(LineRenderer[] line, IntersectionNode localNode, IntersectionNode targetNode)
    {
        for (int i = 0; i < line.Length; i++)
        {
            LineInitialisation(line[i]);
            if (targetNode != null && targetNode.points[i + line.Length] != null) UpdateLinePosition(line[i], localNode.points[i], targetNode.points[i + line.Length]);
        }
    }
}
