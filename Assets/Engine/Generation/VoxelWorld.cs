using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    public static VoxelWorld Instance;

    public Transform player;
    public Material material;

    public int chunkSize = VoxelData.ChunkWidth;
    public int worldHeight = VoxelData.ChunkHeight;
    public int viewDistance = 5;
    public int maxThreads = 2;

    Dictionary<Vector2Int, VoxelChunk> chunks = new();
    HashSet<Vector2Int> building = new();

    Queue<Vector2Int> requestQueue = new();
    Queue<ChunkMeshData> completedQueue = new();

    HashSet<Vector3Int> removedBlocks = new();
    Dictionary<Vector3Int, BlockType> placedBlocks = new();

    int activeThreads = 0;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!player) return;

        Vector2Int pc = new(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        QueueChunks(pc);
        StartThreads();
        ApplyCompleted();
    }

    void QueueChunks(Vector2Int center)
    {
        for (int r = 0; r <= viewDistance; r++)
        {
            for (int x = -r; x <= r; x++)
            for (int z = -r; z <= r; z++)
            {
                if (Mathf.Abs(x) != r && Mathf.Abs(z) != r) continue;

                Vector2Int coord = new(center.x + x, center.y + z);

                if (chunks.ContainsKey(coord)) continue;
                if (building.Contains(coord)) continue;

                requestQueue.Enqueue(coord);
                building.Add(coord);
            }
        }
    }

    void StartThreads()
    {
        while (requestQueue.Count > 0 && activeThreads < maxThreads)
        {
            Vector2Int coord = requestQueue.Dequeue();
            Interlocked.Increment(ref activeThreads);
            ThreadPool.QueueUserWorkItem(_ => BuildChunk(coord));
        }
    }

    void BuildChunk(Vector2Int coord)
    {
        bool[,,] playerMap;
        BlockType[,,] map = GenerateVoxelMap(coord, out playerMap);

        List<Vector3> verts = new();
        List<int> tris = new();
        List<Color> colors = new();

        GreedyMesher.GenerateMesh(map, playerMap, chunkSize, worldHeight, verts, tris, colors);

        ChunkMeshData data = new()
        {
            verts = verts.ToArray(),
            tris = tris.ToArray(),
            colors = colors.ToArray(),
            coord = coord
        };

        lock (completedQueue)
        {
            completedQueue.Enqueue(data);
        }

        Interlocked.Decrement(ref activeThreads);
    }

    void ApplyCompleted()
    {
        lock (completedQueue)
        {
            while (completedQueue.Count > 0)
            {
                var data = completedQueue.Dequeue();

                GameObject obj = new($"Chunk_{data.coord.x}_{data.coord.y}");
                obj.transform.position = new Vector3(data.coord.x * chunkSize, 0, data.coord.y * chunkSize);
                obj.transform.parent = transform;

                var chunk = obj.AddComponent<VoxelChunk>();

                var mr = obj.GetComponent<MeshRenderer>();
                mr.material = material;

                Mesh mesh = new Mesh();
                mesh.vertices = data.verts;
                mesh.triangles = data.tris;
                mesh.colors = data.colors;
                mesh.RecalculateNormals();

                chunk.ApplyMesh(mesh);

                chunks[data.coord] = chunk;
                building.Remove(data.coord);
            }
        }
    }

    BlockType[,,] GenerateVoxelMap(Vector2Int coord, out bool[,,] playerMap)
    {
        BlockType[,,] map = new BlockType[chunkSize, worldHeight, chunkSize];
        playerMap = new bool[chunkSize, worldHeight, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        for (int z = 0; z < chunkSize; z++)
        {
            int worldX = coord.x * chunkSize + x;
            int worldZ = coord.y * chunkSize + z;

            for (int y = 0; y < worldHeight; y++)
            {
                Vector3Int worldPos = new(worldX, y, worldZ);

                if (removedBlocks.Contains(worldPos))
                {
                    map[x, y, z] = BlockType.Air;
                    playerMap[x, y, z] = false;
                    continue;
                }

                if (placedBlocks.TryGetValue(worldPos, out BlockType placed))
                {
                    map[x, y, z] = placed;
                    playerMap[x, y, z] = true;
                    continue;
                }

                map[x, y, z] = TerrainGenerator.GetBlock(worldX, y, worldZ);
                playerMap[x, y, z] = false;
            }
        }

        return map;
    }

    public void RemoveBlock(Vector3 pos)
    {
        Vector3Int p = Vector3Int.FloorToInt(pos);
        removedBlocks.Add(p);
        placedBlocks.Remove(p);
        RebuildChunk(p);
    }

    public void PlaceBlock(Vector3 pos, BlockType type)
    {
        Vector3Int p = Vector3Int.FloorToInt(pos);
        placedBlocks[p] = type;
        removedBlocks.Remove(p);
        RebuildChunk(p);
    }

    void RebuildChunk(Vector3Int p)
    {
        Vector2Int coord = new(
            Mathf.FloorToInt((float)p.x / chunkSize),
            Mathf.FloorToInt((float)p.z / chunkSize)
        );

        if (!building.Contains(coord))
        {
            requestQueue.Enqueue(coord);
            building.Add(coord);
        }
    }
}
