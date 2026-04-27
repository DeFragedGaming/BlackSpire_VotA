using UnityEngine;

public static class Noise
{
    public static float Perlin2D(float x, float z, float scale, float offset)
    {
        return Mathf.PerlinNoise((x + offset) * scale, (z + offset) * scale);
    }
}
