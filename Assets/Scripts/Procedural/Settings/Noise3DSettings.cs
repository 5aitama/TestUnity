using Unity.Mathematics;

[System.Serializable]
public struct Noise3DSettings
{
    /// <summary>
    /// The noise position.
    /// </summary>
    public float3 position;

    /// <summary>
    /// The noise amplitude.
    /// </summary>
    [UnityEngine.Range(0f, 128f)]
    public float amplitude;

    /// <summary>
    /// The noise frequency.
    /// </summary>
    [UnityEngine.Range(0f, 1f)]
    public float frequency;

    /// <summary>
    /// The noise position.
    /// </summary>
    public float3 pos { get => position; }

    /// <summary>
    /// The noise amplitude.
    /// </summary>
    public float amp { get => amplitude; }

    /// <summary>
    /// The noise frequency.
    /// </summary>
    public float frq { get => frequency; }

    /// <summary>
    /// Create new 3D noise settings.
    /// </summary>
    /// <param name="position">The noise position</param>
    /// <param name="amplitude">The noise amplitude</param>
    /// <param name="frequency">The noise frequency</param>
    public Noise3DSettings(in float3 position, in float amplitude, in float frequency)
    {
        this.position  = position;
        this.amplitude = amplitude;
        this.frequency = frequency;
    }
}
