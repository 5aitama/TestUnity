using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public struct ChunkConstructGeometryJob : IJobParallelFor
{
    [ReadOnly]
    public int3 chunkSize;

    [ReadOnly]
    public NativeArray<BlockData> blockData;

    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<float3> vertices;

    [WriteOnly, NativeDisableParallelForRestriction]
    public NativeArray<int> indices;

    [ReadOnly, DeallocateOnJobCompletion]
    public NativeArray<ChunkGeometryInfo> geometryInfos;

    public ChunkConstructGeometryJob(
        in int3 chunkSize, 
        ref NativeArray<BlockData> blockData, 
        ref NativeArray<ChunkGeometryInfo> geometryInfos,
        in NativeArray<ChunkGeometryCount> geometryCount,
        out NativeArray<float3> vertices,
        out NativeArray<int> indices, 
        Allocator allocator)
    {
        vertices    = new NativeArray<float3>(geometryCount[0].vertices, allocator, NativeArrayOptions.UninitializedMemory);
        indices     = new NativeArray<int>(geometryCount[0].indices, allocator, NativeArrayOptions.UninitializedMemory);

        this.chunkSize      = chunkSize + 2;
        this.blockData      = blockData;
        this.vertices       = vertices;
        this.indices        = indices;
        this.geometryInfos  = geometryInfos;

        geometryCount.Dispose();
    }

    public JobHandle ScheduleJob(JobHandle dependsOn = default)
    {
        return this.Schedule(chunkSize.x * chunkSize.y * chunkSize.z, 64, dependsOn);
    }

    public void Execute(int i)
    {
        // Extract the block position.
        int3 p = 0;
        BlockDataDecoder.DecodePosition(blockData[i], ref p);

        if (p.x == 0 || p.y == 0 || p.z == 0 || p.x >= chunkSize.x - 1 || p.y >= chunkSize.y - 1 || p.z >= chunkSize.z - 1) 
            return;

        int id = 0;
        BlockDataDecoder.DecodeID(blockData[i], ref id);

        // Don't calculate the air blocks...
        if (id == 0) return;

        var neighbours = new NativeArray<BlockData>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory); 
        neighbours[0] = blockData[ChunkCalculateGeometryAmountJob.to1D(p - new int3(0, 0, 1), chunkSize)]; // Back
        neighbours[1] = blockData[ChunkCalculateGeometryAmountJob.to1D(p + new int3(1, 0, 0), chunkSize)]; // Right
        neighbours[2] = blockData[ChunkCalculateGeometryAmountJob.to1D(p + new int3(0, 0, 1), chunkSize)]; // Front
        neighbours[3] = blockData[ChunkCalculateGeometryAmountJob.to1D(p - new int3(1, 0, 0), chunkSize)]; // Left
        neighbours[4] = blockData[ChunkCalculateGeometryAmountJob.to1D(p + new int3(0, 1, 0), chunkSize)]; // Top
        neighbours[5] = blockData[ChunkCalculateGeometryAmountJob.to1D(p - new int3(0, 1, 0), chunkSize)]; // Bottom
        
        int tIndex = 0;
        for (var j = 0; j < neighbours.Length; j++) 
        {
            int nID = 0;
            BlockDataDecoder.DecodeID(neighbours[j], ref nID);
            
            if (nID != 0) continue;

            int vOffset = geometryInfos[i].vIndex + tIndex * 4;

            vertices[0 + vOffset] = BlockGeometry.VERTS[BlockGeometry.FACES[j].x] + p;
            vertices[1 + vOffset] = BlockGeometry.VERTS[BlockGeometry.FACES[j].y] + p;
            vertices[2 + vOffset] = BlockGeometry.VERTS[BlockGeometry.FACES[j].z] + p;
            vertices[3 + vOffset] = BlockGeometry.VERTS[BlockGeometry.FACES[j].w] + p;

            int tOffset = geometryInfos[i].tIndex + tIndex * 6;

            indices[0 + tOffset] = (tIndex * 4) + 0 + geometryInfos[i].vIndex;
            indices[1 + tOffset] = (tIndex * 4) + 1 + geometryInfos[i].vIndex;
            indices[2 + tOffset] = (tIndex * 4) + 2 + geometryInfos[i].vIndex;
            indices[3 + tOffset] = (tIndex * 4) + 0 + geometryInfos[i].vIndex;
            indices[4 + tOffset] = (tIndex * 4) + 2 + geometryInfos[i].vIndex;
            indices[5 + tOffset] = (tIndex * 4) + 3 + geometryInfos[i].vIndex;

            tIndex ++;
        }
        
    }
}
