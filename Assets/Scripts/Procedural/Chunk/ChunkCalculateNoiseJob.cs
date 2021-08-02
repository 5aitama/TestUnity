using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public struct ChunkCalculateNoiseJob : IJobParallelFor
{
    /// <summary>
    /// The noise settings.
    /// </summary>
    [ReadOnly]
    private Noise3DSettings noiseSettings;

    /// <summary>
    /// The chunk bounds (need to include extras bounds !)
    /// </summary>
    [ReadOnly]
    private int3 chunkSize;

    /// <summary>
    /// The chunk world position.
    /// </summary>
    [ReadOnly]
    private float3 chunkPos;

    /// <summary>
    /// The block data array.
    /// </summary>
    [WriteOnly]
    private NativeArray<BlockData> blockData;

    /// <summary>
    /// Create new calculate noise job.
    /// </summary>
    /// <param name="noiseSettings">The noise settings</param>
    /// <param name="chunkSize">The size of the chunk</param>
    /// <param name="chunkPos">The position of the chunk in the world</param>
    /// <param name="blockData">The NativeArray that contains all chunk's block data</param>
    public ChunkCalculateNoiseJob(in Noise3DSettings noiseSettings, in int3 chunkSize, in float3 chunkPos, ref NativeArray<BlockData> blockData)
    {
        this.noiseSettings  = noiseSettings;
        this.chunkSize      = chunkSize + 2;
        this.chunkPos       = chunkPos;
        this.blockData      = blockData;
    }

    public ChunkCalculateNoiseJob InitBlockDataNativeArray(ref NativeArray<BlockData> blockData, Allocator allocator)
    {
        if (blockData.IsCreated)
            blockData.Dispose();
        
        blockData = new NativeArray<BlockData>(chunkSize.x * chunkSize.y * chunkSize.z, allocator, NativeArrayOptions.UninitializedMemory);
        this.blockData = blockData;
        return this;
    }

    public JobHandle ScheduleJob(JobHandle dependsOn = default)
    {
        return this.Schedule(chunkSize.x * chunkSize.y * chunkSize.z, 64, dependsOn);
    }

    private float sdBox( float3 p, float3 b, float n )
    {
        var q = math.abs(p - n) - b;
        return math.length(math.max(q, 0.0f)) + math.min(math.max(q.x,math.max(q.y,q.z)),0.0f);
    }

    public void Execute(int i)
    {
        // The block local position.
        var localPos = new int3(i % chunkSize.x, (i / chunkSize.x) % chunkSize.y, i / (chunkSize.x * chunkSize.y));

        // The block world position.
        var worldPos = chunkPos + (float3)localPos - 1;

        // The direction from the planet center to the block.
        var dir = math.normalize(noiseSettings.position - worldPos);

        var planetSurface       = noiseSettings.position + dir * (16f - noiseSettings.amplitude);
        var planetSurfaceNoise  = (noise.snoise(planetSurface * noiseSettings.frequency) + 1f) / 2f * noiseSettings.amplitude;

        var sdBoxValue = sdBox(worldPos - noiseSettings.position, (16f - noiseSettings.amplitude), planetSurfaceNoise);

        // Avoid branching when available ?
        var blockID = 1 - (int)math.max(0f, math.min(1f, sdBoxValue));

        var blockData = new BlockData();
        BlockDataEncoder.EncodePosition(localPos, ref blockData);
        BlockDataEncoder.EncodeID(blockID, ref blockData);

        this.blockData[i] = blockData;
    }
}
