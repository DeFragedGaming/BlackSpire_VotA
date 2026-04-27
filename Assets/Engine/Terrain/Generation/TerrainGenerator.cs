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

            float baseHeight = Mathf.PerlinNoise(wx * 0.02f, wz * 0.02f) * 30f + 20f;
            float ridges = Mathf.Abs(Mathf.PerlinNoise(wx * 0.06f + 100f, wz * 0.06f + 100f) - 0.5f) * 25f;
            float cliffs = Mathf.PerlinNoise(wx * 0.12f + 200f, wz * 0.12f + 200f) * 15f;
            float height = baseHeight + ridges + cliffs;

            float cave3D = Perlin3D(wx * 0.05f + 300f, wy * 0.05f + 300f, wz * 0.05f + 300f);
            float caves = cave3D > 0.35f ? -12f : 0f;

            float corruption = Mathf.PerlinNoise(wx * 0.015f + 500f, wz * 0.015f + 500f);
            float corruptionOffset = corruption > 0.65f ? 18f : 0f;

            float density = (height + corruptionOffset + caves) - wy;
            chunk.density.Set(x, y, z, density);
        }
    }

    static float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        return (xy + yz + zx) / 3f;
    }
}
