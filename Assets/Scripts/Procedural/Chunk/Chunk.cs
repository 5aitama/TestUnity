using UnityEngine;

using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    /// <summary>
    /// The size (width, height, depth) of all chunks.
    /// </summary>
    public static readonly int3 CHUNK_SIZE = 32;

    /// <summary>
    /// The default chunk material.
    /// </summary>
    public static Material material;

    /// <summary>
    /// The chunk's noise settings.
    /// </summary>
    public Noise3DSettings noiseSettings;

    /// <summary>
    /// The chunk's mesh.
    /// </summary>
    private Mesh mesh;

    private NativeArray<BlockData> blockData;

    void Start() 
    {
        if (material == null)
            material = Resources.Load<Material>("Chunk/defaultMaterial");
        
        // Init the chunk mesh and the components...
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BuildChunkMesh();
        }
    }

    void BuildChunkMesh()
    {
        JobHandle handle = default;

        if (!blockData.IsCreated)
        {
            handle = new ChunkCalculateNoiseJob(noiseSettings, CHUNK_SIZE, transform.position, ref blockData)
                .InitBlockDataNativeArray(ref blockData, Allocator.Persistent)
                .ScheduleJob(handle);
        }

        handle = new ChunkCalculateGeometryAmountJob(CHUNK_SIZE, ref blockData, out NativeArray<ChunkGeometryInfo> cgi, out NativeArray<ChunkGeometryCount> cgc, Allocator.TempJob)
            .ScheduleJob(handle);

        handle.Complete();

        new ChunkConstructGeometryJob(CHUNK_SIZE, ref blockData, ref cgi, cgc, out NativeArray<float3> v, out NativeArray<int> i, Allocator.TempJob)
            .ScheduleJob(handle)
            .Complete();

        mesh.Clear();
        mesh.SetVertices(v);
        mesh.SetIndices(i, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();

        v.Dispose();
        i.Dispose();
    }

    void OnDestroy()
    {
        if (blockData.IsCreated)
            blockData.Dispose();
    }
}
