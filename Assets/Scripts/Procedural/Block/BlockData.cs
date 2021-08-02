public struct BlockData 
{
    public int value;

    /// <summary>
    /// Create new block data.
    /// </summary>
    /// <param name="value">The block data value</param>
    public BlockData(in int value = 0) 
    {
        this.value = value;
    }
}