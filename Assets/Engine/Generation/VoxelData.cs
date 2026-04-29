using UnityEngine;

public static class VoxelData
{
    public const int ChunkWidth = 32;
    public const int ChunkHeight = 96;
    public const int ViewDistance = 6;

    public static readonly Vector3[] voxelVerts =
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(1,1,1),
        new Vector3(0,1,1)
    };

    public static readonly int[,] voxelTris =
    {
        {0,3,1,2},
        {5,6,4,7},
        {3,7,2,6},
        {1,5,0,4},
        {4,7,0,3},
        {1,2,5,6}
    };

    public static readonly Vector3[] faceChecks =
    {
        new Vector3(0,0,-1),
        new Vector3(0,0,1),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(-1,0,0),
        new Vector3(1,0,0)
    };
}