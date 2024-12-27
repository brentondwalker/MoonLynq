using UnityEngine;

public class PlaneInfo : MonoBehaviour
{
    public float width;
    public float height;
    public Vector3 vertice;
    public Vector3 verticeLocal;

    public float distanceConvert = 1;

    void Start()
    {
        distanceConvert = 1 / ScenarioScale.staticScale;
    }

    void Update()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        Vector3[] worldVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = transform.TransformPoint(vertices[i]); 
        }

        width = transform.lossyScale.x * 10 ; 
        height = transform.lossyScale.z * 10 ;

        verticeLocal = transform.localPosition + new Vector3(-width / 2 * distanceConvert, 0, -height / 2 * distanceConvert);
        vertice = worldVertices[0];

        //vertice.y = 15.75f;
        //Debug.Log("vertice: " + vertice.x +" "+ vertice.z); 

        foreach (Vector3 vertex in worldVertices)
        {
            //Debug.Log(vertex);
        }

        //Debug.Log($"Plane width = {width}, height = {height}");
    }


}