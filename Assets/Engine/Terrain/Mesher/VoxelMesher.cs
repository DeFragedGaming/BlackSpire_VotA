using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public static class VoxelMesher
{
    private static readonly Vector3Int[] FaceChecks =
    {
        new Vector3Int( 0, 0,-1), // Back
        new Vector3Int( 0, 0, 1), // Front
        new Vector3Int( 0,-1, 0), // Bottom
        new Vector3Int( 0, 1, 0), // Top
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int( 1, 0, 0)  // Right
    };

    private static readonly Vector3[,] FaceVerts =
    {
        // Back
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,0,0)
        },

        // Front
        {
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,0,1)
        },

        // Bottom
        {
            new Vector3(0,0,1),
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,1)
        },

        // Top
        {
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        },

        // Left
        {
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(0,0,0)
        },

        // Right
        {
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(1,0,1)
        }
    };

    private static readonly Vector3[] FaceNormals =
    {
        Vector3.back,
        Vector3.forward,
        Vector3.down,
        Vector3.up,
        Vector3.left,
        Vector3.right
    };

    public static Mesh BuildMesh(VoxelChunk chunk)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        for (int x = 0; x < chunk.sizeX; x++)
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    if (chunk.GetBlock(x,y,z) == 0)
                        continue;

                    for (int f = 0; f < 6; f++)
                    {
                        int nx = x + FaceChecks[f].x;
                        int ny = y + FaceChecks[f].y;
                        int nz = z + FaceChecks[f].z;

                        // Only render if adjacent block is empty
                        if (chunk.GetBlock(nx,ny,nz) != 0)
                            continue;

                        AddFace(
                            f,
                            new Vector3(x,y,z),
                            vertices,
                            triangles,
                            normals,
                            uv
                        );
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles,0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0,uv);

        mesh.RecalculateBounds();

        return mesh;
    }

    private static void AddFace(
        int face,
        Vector3 pos,
        List<Vector3> verts,
        List<int> tris,
        List<Vector3> norms,
        List<Vector2> uv)
    {
        int start = verts.Count;

        for (int i = 0; i < 4; i++)
        {
            verts.Add(pos + FaceVerts[face,i]);
            norms.Add(FaceNormals[face]);
        }

        // Correct winding
        tris.Add(start + 0);
        tris.Add(start + 1);
        tris.Add(start + 2);

        tris.Add(start + 0);
        tris.Add(start + 2);
        tris.Add(start + 3);

        // Standard quad UVs (atlas-safe base)
        uv.Add(new Vector2(0,0));
        uv.Add(new Vector2(0,1));
        uv.Add(new Vector2(1,1));
        uv.Add(new Vector2(1,0));
    }
}