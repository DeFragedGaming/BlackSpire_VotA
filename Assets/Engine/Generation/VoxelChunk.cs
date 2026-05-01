using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public Vector2Int chunkCoord;

    int chunkSize;
    int worldHeight;
    Material mat;
    VoxelWorld world;

    BlockType[,,] voxelMap;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Color> colors = new List<Color>();

    int vertexIndex;

    public void Init(Vector2Int coord, int size, int height, Material material, VoxelWorld worldRef)
    {
        chunkCoord = coord;
        chunkSize = size;
        worldHeight = height;
        mat = material;
        world = worldRef;

        GetComponent<MeshRenderer>().material = mat;

        voxelMap = new BlockType[chunkSize, worldHeight, chunkSize];

        GenerateVoxelMap();
        BuildMesh();
    }

    public void Rebuild()
    {
        GenerateVoxelMap();
        BuildMesh();
    }

    void GenerateVoxelMap()
    {
        for (int x = 0; x < chunkSize; x++)
        for (int z = 0; z < chunkSize; z++)
        {
            int worldX = x + chunkCoord.x * chunkSize;
            int worldZ = z + chunkCoord.y * chunkSize;

            for (int y = 0; y < worldHeight; y++)
            {
                int worldY = y;
                Vector3Int wp = new Vector3Int(worldX, worldY, worldZ);

                if (world != null && world.IsBlockRemoved(wp))
                {
                    voxelMap[x, y, z] = BlockType.Air;
                    continue;
                }

                if (world != null && world.TryGetPlacedBlock(wp, out BlockType placed))
                {
                    voxelMap[x, y, z] = placed;
                    continue;
                }

                voxelMap[x, y, z] = TerrainGenerator.GetBlock(worldX, worldY, worldZ);
            }
        }
    }

    void BuildMesh()
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        vertexIndex = 0;

        for (int x = 0; x < chunkSize; x++)
        for (int y = 0; y < worldHeight; y++)
        for (int z = 0; z < chunkSize; z++)
        {
            if (voxelMap[x, y, z] != BlockType.Air)
                AddVoxel(new Vector3(x, y, z));
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

    void AddVoxel(Vector3 pos)
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 n = pos + VoxelData.faceChecks[i];

            if (!InChunk(n) ||
                voxelMap[(int)n.x, (int)n.y, (int)n.z] == BlockType.Air)
            {
                AddFace(pos, i);
            }
        }
    }

    void AddFace(Vector3 pos, int face)
    {
        BlockType type = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
        Color c = BlockColors.GetColor(type);

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[face, i]]);
            colors.Add(c);
        }

        triangles.Add(vertexIndex + 0);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);

        vertexIndex += 4;
    }

    bool InChunk(Vector3 p)
    {
        return p.x >= 0 && p.x < chunkSize &&
               p.y >= 0 && p.y < worldHeight &&
               p.z >= 0 && p.z < chunkSize;
    }
}
