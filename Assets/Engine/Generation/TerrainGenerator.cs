using UnityEngine;

public static class TerrainGenerator
{
    public const int EngineMinY = 0;
    public const int EngineMaxY = VoxelData.ChunkHeight;

    public const float WorldMinY = -400f;
    public const float WorldMaxY = 400f;

    public const float SeaLevelWorld = 0f;

    public static BlockType GetBlock(int x, int y, int z)
    {
        float worldY = y - 400f;

        if (worldY <= -395f)
            return BlockType.Bedrock;

        float d = Density(x, y, z);

        if (d <= 0f)
        {
            if (worldY <= SeaLevelWorld)
                return BlockType.Water;
            return BlockType.Air;
        }

        float dAbove = Density(x, y + 1, z);
        bool isSurface = d > 0f && dAbove <= 0f;

        if (isSurface)
        {
            if (worldY >= SeaLevelWorld + 2f)
                return BlockType.Grass;

            if (worldY >= SeaLevelWorld - 6f)
                return BlockType.Sand;

            return BlockType.Stone;
        }

        if (worldY > SeaLevelWorld + 4f)
            return BlockType.Dirt;

        return BlockType.Stone;
    }

    static float Density(int x, int y, int z)
    {
        float worldY = y - 400f;

        float continent = Noise2D(x * 0.0008f, z * 0.0008f);
        float erosion   = Noise2D(x * 0.0012f, z * 0.0012f);
        float peaks     = Noise2D(x * 0.0025f, z * 0.0025f);

        float baseHeight = Mathf.Lerp(-180f, 40f, continent);

        float mountainBoost = Mathf.Max(0f, peaks - 0.6f) * 260f;

        float erosionFactor = Mathf.Lerp(1.2f, 0.6f, erosion);

        float targetHeight = (baseHeight + mountainBoost) * erosionFactor;

        float verticalShape = worldY - targetHeight;

        float cave = Noise3D(x * 0.02f, y * 0.02f, z * 0.02f) - 0.55f;

        return -verticalShape - cave * 35f;
    }

    static float Noise2D(float x, float z)
    {
        return Mathf.PerlinNoise(x, z);
    }

    static float Noise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
}
