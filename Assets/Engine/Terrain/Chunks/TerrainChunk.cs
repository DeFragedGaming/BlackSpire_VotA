using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public TerrainDensityField density;
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public void Init(int sx, int sy, int sz)
    {
        sizeX = sx;
        sizeY = sy;
        sizeZ = sz;
        density = new TerrainDensityField(sx + 1, sy + 1, sz + 1);
    }
}
