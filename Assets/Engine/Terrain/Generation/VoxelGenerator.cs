using UnityEngine;

public static class VoxelGenerator
{
    public static void Generate(VoxelChunk chunk, Vector3Int coord, float scale, int baseHeight, int heightRange)
    {
        for (int x = 0; x < chunk.sizeX; x++)
        for (int z = 0; z < chunk.sizeZ; z++)
        {
            float wx = coord.x + x;
            float wz = coord.z + z;

            float n = Mathf.PerlinNoise(wx * scale, wz * scale);
            int h = baseHeight + Mathf.RoundToInt(n * heightRange);

            for (int y = 0; y < chunk.sizeY; y++)
            {
                if (y <= h)
                    chunk.SetBlock(x, y, z, 1);
                else
                    chunk.SetBlock(x, y, z, 0);
            }
        }
    }
}
