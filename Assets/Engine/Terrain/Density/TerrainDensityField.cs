using UnityEngine;

public class TerrainDensityField
{
    public readonly int sizeX;
    public readonly int sizeY;
    public readonly int sizeZ;
    public float[,,] density;

    public TerrainDensityField(int sx, int sy, int sz)
    {
        sizeX = sx;
        sizeY = sy;
        sizeZ = sz;
        density = new float[sx, sy, sz];
    }

    public float Get(int x, int y, int z)
    {
        return density[x, y, z];
    }

    public void Set(int x, int y, int z, float value)
    {
        density[x, y, z] = value;
    }
}
