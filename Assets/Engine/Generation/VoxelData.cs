using UnityEngine;

public static class VoxelData
{
    public const int ChunkWidth = 16;
    public const int ChunkHeight = 128;

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(1,1,1),
        new Vector3(0,1,1),
    };

    public static readonly int[,] voxelTris = new int[6, 4]
    {
        {0,3,1,2}, // Back
        {5,6,4,7}, // Front
        {3,7,2,6}, // Top
        {1,5,0,4}, // Bottom
        {4,7,0,3}, // Left
        {1,2,5,6}  // Right
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0,0,-1),
        new Vector3(0,0,1),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(-1,0,0),
        new Vector3(1,0,0)
    };
}