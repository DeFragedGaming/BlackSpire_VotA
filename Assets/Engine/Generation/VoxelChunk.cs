using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public Vector2Int chunkCoord;

    BlockType[,,] voxelMap;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Color> colors = new();

    int vertexIndex;

    public void Init(Vector2Int coord)
    {
        chunkCoord = coord;

        voxelMap = new BlockType[
            VoxelData.ChunkWidth,
            VoxelData.ChunkHeight,
            VoxelData.ChunkWidth
        ];

        GenerateVoxelMap();
        BuildMesh();
    }

    void GenerateVoxelMap()
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        for (int z = 0; z < VoxelData.ChunkWidth; z++)
        {
            int worldX = x + chunkCoord.x * VoxelData.ChunkWidth;
            int worldZ = z + chunkCoord.y * VoxelData.ChunkWidth;

            float heightNoise = Mathf.PerlinNoise(worldX * .03f, worldZ * .03f) * 20f;
            int height = Mathf.FloorToInt(40 + heightNoise);

            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                if (y > height)
                    voxelMap[x, y, z] = BlockType.Air;
                else if (y == height)
                    voxelMap[x, y, z] = BlockType.Grass;
                else if (y > height - 4)
                    voxelMap[x, y, z] = BlockType.Dirt;
                else
                {
                    float oreNoise = Mathf.PerlinNoise(worldX * .1f, worldZ * .1f);

                    if (oreNoise > 0.75f && y < 50)
                        voxelMap[x, y, z] = BlockType.IronOre;
                    else if (oreNoise > 0.65f && y < 40)
                        voxelMap[x, y, z] = BlockType.CoalOre;
                    else
                        voxelMap[x, y, z] = BlockType.Stone;
                }
            }
        }
    }

    void BuildMesh()
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        vertexIndex = 0;

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        for (int z = 0; z < VoxelData.ChunkWidth; z++)
        {
            if (voxelMap[x, y, z] != BlockType.Air)
                AddVoxelData(new Vector3(x, y, z));
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider col = GetComponent<MeshCollider>();
        col.sharedMesh = null;
        col.sharedMesh = mesh;
    }

    void AddVoxelData(Vector3 pos)
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 neighbor = pos + VoxelData.faceChecks[i];

            if (!IsVoxelInChunk(neighbor) ||
                voxelMap[(int)neighbor.x, (int)neighbor.y, (int)neighbor.z] == BlockType.Air)
            {
                AddFace(pos, i);
            }
        }
    }

    void AddFace(Vector3 pos, int faceIndex)
    {
        Color col = BlockColors.GetColor(
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z]);

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[faceIndex, i]]);
            colors.Add(col);
        }

        triangles.Add(vertexIndex + 0);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);

        vertexIndex += 4;
    }

    bool IsVoxelInChunk(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < VoxelData.ChunkWidth &&
               pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
               pos.z >= 0 && pos.z < VoxelData.ChunkWidth;
    }

    public void RemoveBlock(Vector3 hitPoint, Vector3 normal)
    {
        Vector3 local = hitPoint - transform.position - normal * 0.01f;

        int x = Mathf.FloorToInt(local.x);
        int y = Mathf.FloorToInt(local.y);
        int z = Mathf.FloorToInt(local.z);

        if (!IsVoxelInChunk(new Vector3(x, y, z)))
            return;

        voxelMap[x, y, z] = BlockType.Air;
        BuildMesh();
    }

    public void PlaceBlock(Vector3 hitPoint, Vector3 normal, BlockType type)
    {
        Vector3 local = hitPoint - transform.position + normal * 0.01f;

        int x = Mathf.FloorToInt(local.x);
        int y = Mathf.FloorToInt(local.y);
        int z = Mathf.FloorToInt(local.z);

        if (!IsVoxelInChunk(new Vector3(x, y, z)))
            return;

        voxelMap[x, y, z] = type;
        BuildMesh();
    }
}