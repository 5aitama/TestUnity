using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

public struct ChunkGeometryInfo 
{
    public int vIndex; // The vertex index.
    public int tIndex; // The index index.
    public int fIndex; // The face index.
    public int bIndex; // The block index.
}

public struct ChunkGeometryCount 
{
    /// <summary>
    /// The amount of vertices.
    /// </summary>
    public int vertices;

    /// <summary>
    /// The amount of indices.
    /// </summary>
    public int indices;

    public static ChunkGeometryCount operator +(ChunkGeometryCount lhs, int2 rhs)
    {
        return new ChunkGeometryCount {
            vertices = lhs.vertices + rhs.x,
            indices  = lhs.indices + rhs.y,
        };
    }
}

[BurstCompile]
public struct ChunkCalculateGeometryAmountJob : IJob
{
    /// <summary>
    /// Chunk bounds.
    /// </summary>
    [ReadOnly]
    public int3 chunkSize;

    /// <summary>
    /// Chunk block data array.
    /// </summary>
    [ReadOnly]
    public NativeArray<BlockData> blockData;

    /// <summary>
    /// The pre calculated geometry info for the chunk.
    /// </summary>
    [WriteOnly]
    public NativeArray<ChunkGeometryInfo> geometryInfos;

    /// <summary>
    /// Store the amount of vertices & indices.
    /// </summary>
    public NativeArray<ChunkGeometryCount> geometryCount;

    public ChunkCalculateGeometryAmountJob(
        in int3 chunkSize, 
        ref NativeArray<BlockData> blockData, 
        out NativeArray<ChunkGeometryInfo>  geometryInfos, 
        out NativeArray<ChunkGeometryCount> geometryCount,
        Allocator allocator)
    {
        geometryInfos = new NativeArray<ChunkGeometryInfo>(blockData.Length, allocator, NativeArrayOptions.UninitializedMemory);
        geometryCount = new NativeArray<ChunkGeometryCount>(2, allocator, NativeArrayOptions.ClearMemory);

        this.blockData = blockData;
        this.chunkSize = chunkSize + 2;
        this.geometryInfos = geometryInfos;
        this.geometryCount = geometryCount;
    }

    public JobHandle ScheduleJob(JobHandle dependsOn = default)
    {
        return this.Schedule(dependsOn);
    }

    public void Execute()
    {
        int gIndex = 0;

        for (int i = 0; i < blockData.Length; i++) {
            int3 blockPos = 0;
            BlockDataDecoder.DecodePosition(blockData[i], ref blockPos);
            
            int blockID = 0;
            BlockDataDecoder.DecodeID(blockData[i], ref blockID);

            // Default geometry info...
            geometryInfos[i] = new ChunkGeometryInfo {
                vIndex = -1,
                tIndex = -1,
                fIndex = -1,
                bIndex = -1,
            };

            bool3 minBounds = blockPos == new int3(0);
            bool3 maxBounds = blockPos >= chunkSize - 1;

            if (minBounds.x || minBounds.y || minBounds.z || maxBounds.x || maxBounds.y || maxBounds.z || blockID == 0) 
                continue;

            var neighbours = new NativeArray<BlockData>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory); 
            neighbours[0] = blockData[to1D(blockPos - new int3(0, 0, 1), chunkSize)]; // Back
            neighbours[1] = blockData[to1D(blockPos + new int3(1, 0, 0), chunkSize)]; // Right
            neighbours[2] = blockData[to1D(blockPos + new int3(0, 0, 1), chunkSize)]; // Front
            neighbours[3] = blockData[to1D(blockPos - new int3(1, 0, 0), chunkSize)]; // Left
            neighbours[4] = blockData[to1D(blockPos + new int3(0, 1, 0), chunkSize)]; // Top
            neighbours[5] = blockData[to1D(blockPos - new int3(0, 1, 0), chunkSize)]; // Bottom
            
            var geometryInfo = new ChunkGeometryInfo {
                fIndex = gIndex,
                vIndex = gIndex * 4,
                tIndex = gIndex * 6,
                bIndex = i,
            };

            for (var j = 0; j < neighbours.Length; j++)
            {
                var nBlockID = 0;
                BlockDataDecoder.DecodeID(neighbours[j], ref nBlockID);

                if (nBlockID == 0) {
                    geometryCount[0] += new int2(4, 6);
                    gIndex++;
                }
            }

            geometryInfos[i] = geometryInfo;
        }
    }

    public static int to1D(in int3 xyz, in int3 size) 
        => xyz.x + xyz.y * size.x + xyz.z * size.x * size.y;
}
