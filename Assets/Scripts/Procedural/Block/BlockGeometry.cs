using Unity.Mathematics;

public struct BlockGeometry
{
    /// <summary>
    /// Contains the vertices of a block at
    /// (0, 0, 0) with size of 1 (0.5 from 
    /// the center)
    /// </summary>
    public static readonly float3[] VERTS = {
        new float3(-.5f, -.5f, -.5f),
        new float3(-.5f,  .5f, -.5f),
        new float3( .5f,  .5f, -.5f),
        new float3( .5f, -.5f, -.5f),

        new float3(-.5f, -.5f,  .5f),
        new float3(-.5f,  .5f,  .5f),
        new float3( .5f,  .5f,  .5f),
        new float3( .5f, -.5f,  .5f),
    };

    /// <summary>
    /// Contains the index of vertex for each
    /// block faces.
    /// </summary>
    public static readonly int4[] FACES = {
        new int4(0, 1, 2, 3), // z- face
        new int4(3, 2, 6, 7), // x+ face
        new int4(7, 6, 5, 4), // z+ face
        new int4(4, 5, 1, 0), // x- face
        new int4(1, 5, 6, 2), // y+ face
        new int4(4, 0, 3, 7), // y- face
    };
}
