using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    public static VoxelWorld Instance;

    public Transform player;
    public Material material;

    public int chunkSize = VoxelData.ChunkWidth;
    public int worldHeight = VoxelData.ChunkHeight;
    public int viewDistance = 5;

    Dictionary<Vector2Int, VoxelChunk> chunks = new Dictionary<Vector2Int, VoxelChunk>();

    HashSet<Vector3Int> removedBlocks = new HashSet<Vector3Int>();
    Dictionary<Vector3Int, BlockType> placedBlocks = new Dictionary<Vector3Int, BlockType>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int pc = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        for (int x = -viewDistance; x <= viewDistance; x++)
        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            Vector2Int coord = new Vector2Int(pc.x + x, pc.y + z);

            if (!chunks.ContainsKey(coord))
                CreateChunk(coord);
        }
    }

    void CreateChunk(Vector2Int coord)
    {
        GameObject obj = new GameObject($"Chunk_{coord.x}_{coord.y}");
        obj.transform.position = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
        obj.transform.parent = transform;

        VoxelChunk chunk = obj.AddComponent<VoxelChunk>();
        chunk.Init(coord, chunkSize, worldHeight, material, this);

        chunks.Add(coord, chunk);
    }

    public void RemoveBlock(Vector3 pos)
    {
        Vector3Int p = Vector3Int.FloorToInt(pos);
        removedBlocks.Add(p);
        placedBlocks.Remove(p);
        RefreshChunk(p);
    }

    public void PlaceBlock(Vector3 pos, BlockType type)
    {
        Vector3Int p = Vector3Int.FloorToInt(pos);
        placedBlocks[p] = type;
        removedBlocks.Remove(p);
        RefreshChunk(p);
    }

    public bool IsBlockRemoved(Vector3Int p) => removedBlocks.Contains(p);

    public bool TryGetPlacedBlock(Vector3Int p, out BlockType type) => placedBlocks.TryGetValue(p, out type);

    void RefreshChunk(Vector3Int p)
    {
        Vector2Int c = new Vector2Int(
            Mathf.FloorToInt((float)p.x / chunkSize),
            Mathf.FloorToInt((float)p.z / chunkSize)
        );

        if (chunks.TryGetValue(c, out VoxelChunk chunk))
            chunk.Rebuild();
    }
}
