using UnityEngine;

public static class TerrainGenerator
{
    public const int MinY = 0;
    public const int MaxY = VoxelData.ChunkHeight;

    public const int HellTop = 200;
    public const int BedrockTop = 4;

    public const int SurfaceBase = 400;
    public const int SeaLevel = 380;

    public static BiomeType GetSurfaceBiome(int x, int z)
    {
        float n = Mathf.PerlinNoise(x * 0.0008f, z * 0.0008f);

        if (n < 0.3f)  return BiomeType.Ocean;
        if (n < 0.5f)  return BiomeType.Plains;
        if (n < 0.75f) return BiomeType.Hills;
        return BiomeType.Mountains;
    }

    public static int GetSurfaceHeight(int x, int z, BiomeType biome)
    {
        float baseHeight = SurfaceBase;

        switch (biome)
        {
            case BiomeType.Ocean:
            {
                float c = Mathf.PerlinNoise(x * 0.0015f, z * 0.0015f);
                return Mathf.RoundToInt(baseHeight - 40f - c * 40f);
            }

            case BiomeType.Plains:
            {
                float h = Mathf.PerlinNoise(x * 0.004f, z * 0.004f);
                return Mathf.RoundToInt(baseHeight + (h - 0.5f) * 20f);
            }

            case BiomeType.Hills:
            {
                float h = Mathf.PerlinNoise(x * 0.003f, z * 0.003f);
                return Mathf.RoundToInt(baseHeight + (h - 0.5f) * 60f);
            }

            case BiomeType.Mountains:
            default:
            {
                float h1 = Mathf.PerlinNoise(x * 0.002f, z * 0.002f);
                float h2 = Mathf.PerlinNoise(x * 0.008f, z * 0.008f);
                float h = h1 * 0.7f + h2 * 0.3f;
                return Mathf.RoundToInt(baseHeight + (h - 0.4f) * 140f);
            }
        }
    }

    public static BiomeType GetHellBiome(int x, int z)
    {
        float n = Mathf.PerlinNoise(x * 0.0012f + 5000f, z * 0.0012f + 8000f);

        if (n < 0.33f)  return BiomeType.HellAsh;
        if (n < 0.66f)  return BiomeType.HellFlesh;
        return BiomeType.HellObsidian;
    }

    public static bool IsOverworldCave(int x, int y, int z, BiomeType biome)
    {
        float n1 = Mathf.PerlinNoise(x * 0.03f + y * 0.015f, z * 0.03f + y * 0.015f);
        float n2 = Mathf.PerlinNoise(x * 0.02f + 1000f, z * 0.02f + 2000f);
        float v = n1 * 0.6f + n2 * 0.4f;

        float threshold = 0.6f;

        if (biome == BiomeType.Mountains)
            threshold = 0.55f;
        else if (biome == BiomeType.Ocean)
            threshold = 0.65f;

        return v > threshold;
    }

    public static bool IsHellCave(int x, int y, int z, BiomeType hellBiome)
    {
        float n1 = Mathf.PerlinNoise(x * 0.04f + y * 0.02f + 3000f, z * 0.04f + y * 0.02f + 6000f);
        float n2 = Mathf.PerlinNoise(x * 0.015f + 9000f, z * 0.015f + 12000f);
        float v = n1 * 0.7f + n2 * 0.3f;

        float threshold = 0.58f;

        if (hellBiome == BiomeType.HellObsidian)
            threshold = 0.54f;
        else if (hellBiome == BiomeType.HellFlesh)
            threshold = 0.62f;

        return v > threshold;
    }

    public static BlockType GetBlock(int x, int y, int z)
    {
        if (y <= BedrockTop)
            return BlockType.Bedrock;

        if (y < HellTop)
        {
            BiomeType hellBiome = GetHellBiome(x, z);

            if (IsHellCave(x, y, z, hellBiome) && y > BedrockTop + 4)
                return BlockType.Air;

            switch (hellBiome)
            {
                case BiomeType.HellAsh:
                    return BlockType.Stone;
                case BiomeType.HellFlesh:
                    return BlockType.Dirt;
                case BiomeType.HellObsidian:
                default:
                    return BlockType.Stone;
            }
        }

        BiomeType surfaceBiome = GetSurfaceBiome(x, z);
        int surfaceHeight = GetSurfaceHeight(x, z, surfaceBiome);

        if (y > surfaceHeight)
        {
            if (y <= SeaLevel &&
                (surfaceBiome == BiomeType.Ocean || surfaceHeight < SeaLevel - 6))
                return BlockType.Water;

            return BlockType.Air;
        }

        if (IsOverworldCave(x, y, z, surfaceBiome) && y < surfaceHeight - 4)
            return BlockType.Air;

        int depth = surfaceHeight - y;

        if (surfaceBiome == BiomeType.Ocean)
        {
            if (depth <= 3) return BlockType.Sand;
            return BlockType.Stone;
        }

        if (depth == 0)
            return BlockType.Grass;

        if (depth < 4)
            return BlockType.Dirt;

        float oreNoise = Mathf.PerlinNoise(x * 0.05f, z * 0.05f);

        if (y < SeaLevel - 40)
        {
            if (oreNoise > 0.82f)
                return BlockType.IronOre;
            if (oreNoise > 0.76f)
                return BlockType.CoalOre;
        }

        return BlockType.Stone;
    }
}
