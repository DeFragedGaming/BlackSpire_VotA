using System.Collections.Generic;
using UnityEngine;

public static class GreedyMesher
{
    struct MaskCell
    {
        public BlockType block;
        public bool backFace;
        public bool player;

        public MaskCell(BlockType b, bool back, bool p)
        {
            block = b;
            backFace = back;
            player = p;
        }
    }

    public static void GenerateMesh(BlockType[,,] map, bool[,,] playerMap, int chunkSize, int chunkHeight,
        List<Vector3> verts, List<int> tris, List<Color> colors)
    {
        verts.Clear();
        tris.Clear();
        colors.Clear();

        int vertexIndex = 0;

        int[][] dims = {
            new int[] { 0, 1, 2 },
            new int[] { 1, 2, 0 },
            new int[] { 2, 0, 1 }
        };

        int[] size = { chunkSize, chunkHeight, chunkSize };

        for (int d = 0; d < 3; d++)
        {
            int i = dims[d][0];
            int j = dims[d][1];
            int k = dims[d][2];

            int[] q = { 0, 0, 0 };
            q[i] = 1;

            MaskCell[,] mask = new MaskCell[size[j], size[k]];

            for (int x = -1; x < size[i]; x++)
            {
                for (int y = 0; y < size[j]; y++)
                for (int z = 0; z < size[k]; z++)
                {
                    BlockType a = BlockType.Air;
                    BlockType b = BlockType.Air;
                    bool aPlayer = false;
                    bool bPlayer = false;

                    if (x >= 0)
                    {
                        int[] p = { 0, 0, 0 };
                        p[i] = x; p[j] = y; p[k] = z;
                        a = map[p[0], p[1], p[2]];
                        aPlayer = playerMap[p[0], p[1], p[2]];
                    }

                    if (x < size[i] - 1)
                    {
                        int[] p = { 0, 0, 0 };
                        p[i] = x + 1; p[j] = y; p[k] = z;
                        b = map[p[0], p[1], p[2]];
                        bPlayer = playerMap[p[0], p[1], p[2]];
                    }

                    if (a != BlockType.Air && b == BlockType.Air)
                    {
                        mask[y, z] = new MaskCell(a, false, aPlayer);
                    }
                    else if (a == BlockType.Air && b != BlockType.Air)
                    {
                        mask[y, z] = new MaskCell(b, true, bPlayer);
                    }
                    else
                    {
                        mask[y, z] = new MaskCell(BlockType.Air, false, false);
                    }
                }

                for (int y = 0; y < size[j]; y++)
                {
                    for (int z = 0; z < size[k];)
                    {
                        MaskCell cell = mask[y, z];

                        if (cell.block == BlockType.Air)
                        {
                            z++;
                            continue;
                        }

                        int width = 1;
                        while (z + width < size[k])
                        {
                            var next = mask[y, z + width];
                            if (next.block != cell.block ||
                                next.backFace != cell.backFace ||
                                next.player != cell.player)
                                break;
                            width++;
                        }

                        int height = 1;
                        bool done = false;

                        while (y + height < size[j] && !done)
                        {
                            for (int w = 0; w < width; w++)
                            {
                                var next = mask[y + height, z + w];
                                if (next.block != cell.block ||
                                    next.backFace != cell.backFace ||
                                    next.player != cell.player)
                                {
                                    done = true;
                                    break;
                                }
                            }
                            if (!done) height++;
                        }

                        int[] du = { 0, 0, 0 };
                        int[] dv = { 0, 0, 0 };

                        du[j] = height;
                        dv[k] = width;

                        int[] p0 = { 0, 0, 0 };
                        p0[i] = x + 1;
                        p0[j] = y;
                        p0[k] = z;

                        Vector3 v0 = new Vector3(p0[0], p0[1], p0[2]);
                        Vector3 v1 = v0 + new Vector3(du[0], du[1], du[2]);
                        Vector3 v2 = v0 + new Vector3(dv[0], dv[1], dv[2]);
                        Vector3 v3 = v1 + new Vector3(dv[0], dv[1], dv[2]);

                        Color c = BlockColors.GetColor(cell.block);

                        verts.Add(v0); colors.Add(c);
                        verts.Add(v1); colors.Add(c);
                        verts.Add(v2); colors.Add(c);
                        verts.Add(v3); colors.Add(c);

                        if (!cell.backFace)
                        {
                            tris.Add(vertexIndex + 0);
                            tris.Add(vertexIndex + 1);
                            tris.Add(vertexIndex + 2);
                            tris.Add(vertexIndex + 2);
                            tris.Add(vertexIndex + 1);
                            tris.Add(vertexIndex + 3);
                        }
                        else
                        {
                            tris.Add(vertexIndex + 0);
                            tris.Add(vertexIndex + 2);
                            tris.Add(vertexIndex + 1);
                            tris.Add(vertexIndex + 2);
                            tris.Add(vertexIndex + 3);
                            tris.Add(vertexIndex + 1);
                        }

                        vertexIndex += 4;

                        for (int yy = 0; yy < height; yy++)
                        for (int ww = 0; ww < width; ww++)
                            mask[y + yy, z + ww] = new MaskCell(BlockType.Air, false, false);

                        z += width;
                    }
                }
            }
        }
    }
}
