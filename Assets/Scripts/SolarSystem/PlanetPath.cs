
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlanetPath : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    public float2 center    = 0f;
    public float  radius    = 10f;
    public float  width     = 1f;

    public float2[] points { get; private set; } = new float2[4];

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        meshFilter.mesh = mesh;
    }

    private void Start()
    {
        CalculatePoints();
        GenerateMesh();
    }

    private void CalculatePoints()
    {
        points = new float2[] {
            new float2(-radius, -radius),
            new float2(-radius,  radius),
            new float2( radius,  radius),
            new float2( radius, -radius),
        };
    }

    private void GenerateMesh()
    {
        var v = new Vector3[8] {
            new float3(points[0], 0),
            new float3(points[1], 0),
            new float3(points[2], 0),
            new float3(points[3], 0),

            new float3(points[0] + (points[0] / radius) * width, 0),
            new float3(points[1] + (points[1] / radius) * width, 0),
            new float3(points[2] + (points[2] / radius) * width, 0),
            new float3(points[3] + (points[3] / radius) * width, 0),
        };

        var i = new int[24] {
            0, 4, 5,    0, 5, 1,
            1, 5, 6,    1, 6, 2,
            2, 6, 7,    2, 7, 3,
            3, 7, 4,    3, 4, 0,
        };

        mesh.Clear();
        mesh.SetVertices(v);
        mesh.SetIndices(i, MeshTopology.Triangles, 0);
        
    }
}
