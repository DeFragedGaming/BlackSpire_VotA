using UnityEngine;
using System.Collections.Generic;

public static class DualContourMesher
{
    public static Mesh BuildMesh(TerrainChunk chunk)
    {
        int sx = chunk.sizeX;
        int sy = chunk.sizeY;
        int sz = chunk.sizeZ;

        List<Vector3> verts = new();
        List<int> tris = new();

        
        int[,,] vIndex = new int[sx, sy, sz];

        
        for (int x = 0; x < sx; x++)
        for (int y = 0; y < sy; y++)
        for (int z = 0; z < sz; z++)
        {
            List<HermiteSample> samples = CollectHermiteSamples(chunk, x, y, z);

            if (samples.Count == 0)
            {
                vIndex[x, y, z] = -1;
                continue;
            }

            Vector3 center = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
            Vector3 v = QEFSolver.Solve(samples, center);

            vIndex[x, y, z] = verts.Count;
            verts.Add(v);
        }

        
        for (int x = 0; x < sx - 1; x++)
        for (int y = 0; y < sy - 1; y++)
        for (int z = 0; z < sz - 1; z++)
        {
            int v000 = vIndex[x, y, z];
            if (v000 < 0) continue;

            // +X face
            int v100 = vIndex[x + 1, y, z];
            if (v100 >= 0)
            {
                AddQuad(tris, v000, v100, vIndex[x, y + 1, z], vIndex[x + 1, y + 1, z]);
            }

            // +Y face
            int v010 = vIndex[x, y + 1, z];
            if (v010 >= 0)
            {
                AddQuad(tris, v000, v010, vIndex[x + 1, y, z], vIndex[x + 1, y + 1, z]);
            }

            // +Z face
            int v001 = vIndex[x, y, z + 1];
            if (v001 >= 0)
            {
                AddQuad(tris, v000, v001, vIndex[x + 1, y, z], vIndex[x + 1, y, z + 1]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private static void AddQuad(List<int> tris, int a, int b, int c, int d)
    {
        if (a < 0 || b < 0 || c < 0 || d < 0)
            return;

        tris.Add(a); tris.Add(b); tris.Add(c);
        tris.Add(c); tris.Add(b); tris.Add(d);
    }

    private static List<HermiteSample> CollectHermiteSamples(TerrainChunk chunk, int x, int y, int z)
    {
        List<HermiteSample> samples = new();

        Vector3Int p000 = new(x, y, z);
        Vector3Int p100 = new(x + 1, y, z);
        Vector3Int p010 = new(x, y + 1, z);
        Vector3Int p001 = new(x, y, z + 1);

        HermiteSample s;

        if (HermiteExtractor.TrySampleEdge(chunk.density, p000, p100, out s)) samples.Add(s);
        if (HermiteExtractor.TrySampleEdge(chunk.density, p000, p010, out s)) samples.Add(s);
        if (HermiteExtractor.TrySampleEdge(chunk.density, p000, p001, out s)) samples.Add(s);

        return samples;
    }
}
