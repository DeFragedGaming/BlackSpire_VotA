using UnityEngine;

public static class TerrainGenerator
{
    public static void Generate(TerrainChunk chunk, Vector3Int worldPos)
    {
        for (int x = 0; x <= chunk.sizeX; x++)
        for (int y = 0; y <= chunk.sizeY; y++)
        for (int z = 0; z <= chunk.sizeZ; z++)
        {
            float wx = worldPos.x + x;
            float wy = worldPos.y + y;
            float wz = worldPos.z + z;

            float height = Mathf.PerlinNoise(wx * 0.01f, wz * 0.01f) * 40f + 20f;

            float density = wy - height;

            chunk.density.Set(x, y, z, density);
        }
    }
}
