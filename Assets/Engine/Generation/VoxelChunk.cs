using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(
typeof(MeshFilter),
typeof(MeshRenderer),
typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public Vector2Int chunkCoord;

    BlockType[,,] voxelMap;

    List<Vector3> vertices = new();
    List<int> triangles = new();

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

            float continent = Mathf.PerlinNoise(worldX * .01f, worldZ * .01f) * 30f;
            float hills = Mathf.PerlinNoise(worldX * .04f, worldZ * .04f) * 10f;

            int height = Mathf.FloorToInt(30 + continent + hills);

            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                if (y > height)
                    voxelMap[x, y, z] = BlockType.Air;
                else if (y == height)
                    voxelMap[x, y, z] = BlockType.Grass;
                else if (y > height - 5)
                    voxelMap[x, y, z] = BlockType.Dirt;
                else
                    voxelMap[x, y, z] = BlockType.Stone;
            }
        }
    }

    public void BuildMesh()
    {
        vertices.Clear();
        triangles.Clear();
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

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = null;
        mc.sharedMesh = mesh;
    }

    void AddVoxelData(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (CheckSolid(pos + VoxelData.faceChecks[p]))
                continue;

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);

            for (int i = 0; i < 4; i++)
            {
                vertices.Add(
                    pos +
                    VoxelData.voxelVerts[
                        VoxelData.voxelTris[p, i]
                    ]
                );
            }

            vertexIndex += 4;
        }
    }

    bool CheckSolid(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        if (x < 0 || x >= VoxelData.ChunkWidth ||
            y < 0 || y >= VoxelData.ChunkHeight ||
            z < 0 || z >= VoxelData.ChunkWidth)
            return false;

        return voxelMap[x, y, z] != BlockType.Air;
    }

    // =========================
    // 🧱 MINING SYSTEM (V1)
    // =========================

    public void RemoveBlock(Vector3 worldPos)
    {
        Vector3 local =
            transform.InverseTransformPoint(worldPos);

        int x = Mathf.FloorToInt(local.x);
        int y = Mathf.FloorToInt(local.y);
        int z = Mathf.FloorToInt(local.z);

        if (x < 0 || x >= VoxelData.ChunkWidth ||
            y < 0 || y >= VoxelData.ChunkHeight ||
            z < 0 || z >= VoxelData.ChunkWidth)
            return;

        voxelMap[x, y, z] = BlockType.Air;

        BuildMesh();

        Debug.Log($"Block mined at {x},{y},{z}");
    }
}