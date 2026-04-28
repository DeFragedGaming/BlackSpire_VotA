using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public int sizeX = 16;
    public int sizeY = 128;
    public int sizeZ = 16;

    public int worldX;
    public int worldZ;

    public byte[,,] blocks;

    MeshFilter mf;
    MeshCollider mc;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();

        blocks = new byte[sizeX,sizeY,sizeZ];
    }

    public byte GetBlock(int x,int y,int z)
    {
        if(x<0||y<0||z<0||
           x>=sizeX||y>=sizeY||z>=sizeZ)
            return 0;

        return blocks[x,y,z];
    }

    public void SetBlock(int x,int y,int z, byte id)
    {
        if(x<0||y<0||z<0||
           x>=sizeX||y>=sizeY||z>=sizeZ)
            return;

        blocks[x,y,z]=id;
    }

    public void BuildMesh()
    {
        Mesh mesh = VoxelMesher.BuildMesh(this);

        mf.sharedMesh = mesh;

        mc.sharedMesh = null;
        mc.sharedMesh = mesh;
    }
}