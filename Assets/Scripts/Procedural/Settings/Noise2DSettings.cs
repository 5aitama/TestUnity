using Unity.Mathematics;

public struct Noise2DSettings
{
    /// <summary>
    /// The noise position.
    /// </summary>
    public float2 position;

    /// <summary>
    /// The noise amplitude.
    /// </summary>
    public float amplitude;

    /// <summary>
    /// The noise frequency.
    /// </summary>
    public float frequency;

    /// <summary>
    /// The noise position.
    /// </summary>
    public float2 pos { get => position; }

    /// <summary>
    /// The noise amplitude.
    /// </summary>
    public float amp { get => amplitude; }

    /// <summary>
    /// The noise frequency.
    /// </summary>
    public float frq { get => frequency; }

    /// <summary>
    /// Create new 2D noise settings.
    /// </summary>
    /// <param name="position">The noise position</param>
    /// <param name="amplitude">The noise amplitude</param>
    /// <param name="frequency">The noise frequency</param>
    public Noise2DSettings(in float2 position, in float amplitude, in float frequency)
    {
        this.position  = position;
        this.amplitude = amplitude;
        this.frequency = frequency;
    }
}
