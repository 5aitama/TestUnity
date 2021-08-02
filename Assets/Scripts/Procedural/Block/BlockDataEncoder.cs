using Unity.Mathematics;
using Unity.Collections;

/// <summary>
/// Set of methods to easily encode block data.
/// </summary>
public struct BlockDataEncoder
{
    /// <summary>
    /// Encode the x component of a position into a block data.
    /// </summary>
    /// <param name="x">The x component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodePositionComponentX(in int x, ref BlockData data)
        => data.value |= (x & 0x3F);

    /// <summary>
    /// Encode the y component of a position into a block data.
    /// </summary>
    /// <param name="y">The y component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodePositionComponentY(in int y, ref BlockData data)
        => data.value |= (y & 0x3F) << 0x06;

    /// <summary>
    /// Encode the z component of a position into a block data.
    /// </summary>
    /// <param name="z">The z component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodePositionComponentZ(in int z, ref BlockData data)
        => data.value |= (z & 0x3F) << 0x0C;

    /// <summary>
    /// Encode the given position into a block data.
    /// </summary>
    /// <param name="pos">The position to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodePosition(in int3 pos, ref BlockData data) 
    {
        EncodePositionComponentX(pos.x, ref data);
        EncodePositionComponentY(pos.y, ref data);
        EncodePositionComponentZ(pos.z, ref data);
    }

    /// <summary>
    /// Encode the given position into a block data.
    /// </summary>
    /// <param name="pos">The position to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodePosition(in int2 pos, ref BlockData data) 
    {
        EncodePositionComponentX(pos.x, ref data);
        EncodePositionComponentY(pos.y, ref data);
    }

    /// <summary>
    /// Encode the given id into a block data.
    /// </summary>
    /// <param name="id">The ID to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void EncodeID(in int id, ref BlockData data)
        => data.value |= (id & 0xFF) << 0x12;
}
