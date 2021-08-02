using Unity.Mathematics;
using Unity.Collections;

/// <summary>
/// Set of methods to easily decode block data.
/// </summary>
public struct BlockDataDecoder
{
    /// <summary>
    /// Encode the x component of a position into a block data.
    /// </summary>
    /// <param name="x">The x component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodePositionComponentX(in BlockData data, ref int x)
        => x = (data.value & 0x3F);

    /// <summary>
    /// Encode the y component of a position into a block data.
    /// </summary>
    /// <param name="y">The y component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodePositionComponentY(in BlockData data, ref int y)
        => y = (data.value >> 0x06) & 0x3F;

    /// <summary>
    /// Encode the z component of a position into a block data.
    /// </summary>
    /// <param name="z">The z component value to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodePositionComponentZ(in BlockData data, ref int z)
        => z = (data.value >> 0x0C) & 0x3F;

    /// <summary>
    /// Encode the given position into a block data.
    /// </summary>
    /// <param name="pos">The position to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodePosition(in BlockData data, ref int3 pos) 
    {
        DecodePositionComponentX(data, ref pos.x);
        DecodePositionComponentY(data, ref pos.y);
        DecodePositionComponentZ(data, ref pos.z);
    }

    /// <summary>
    /// Decode the given position into a block data.
    /// </summary>
    /// <param name="pos">The position to encode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodePosition(in BlockData data, ref int2 pos) 
    {
        DecodePositionComponentX(data, ref pos.x);
        DecodePositionComponentY(data, ref pos.y);
    }

    /// <summary>
    /// Decode the given id into a block data.
    /// </summary>
    /// <param name="id">The ID to decode</param>
    /// <param name="data">The block data</param>
    [BurstCompatible]
    public static void DecodeID(in BlockData data, ref int id)
        => id = (data.value >> 0x12) & 0xFF;
}