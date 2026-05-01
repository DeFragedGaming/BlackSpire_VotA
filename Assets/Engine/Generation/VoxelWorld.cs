using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    public static VoxelWorld Instance;

    public Dictionary<Vector2Int, VoxelChunk> chunks = new();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterChunk(Vector2Int coord, VoxelChunk chunk)
    {
        if (!chunks.ContainsKey(coord))
            chunks.Add(coord, chunk);
    }

    public VoxelChunk GetChunkFromWorldPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(worldPos.z / VoxelData.ChunkWidth);

        Vector2Int coord = new Vector2Int(x, z);

        if (chunks.TryGetValue(coord, out VoxelChunk chunk))
            return chunk;

        return null;
    }

    public void RemoveBlock(Vector3 worldPos, Vector3 normal)
    {
        VoxelChunk chunk = GetChunkFromWorldPos(worldPos);
        if (chunk == null) return;

        chunk.RemoveBlock(worldPos, normal);
    }

    public void PlaceBlock(Vector3 worldPos, Vector3 normal, BlockType type)
    {
        VoxelChunk chunk = GetChunkFromWorldPos(worldPos + normal * 0.01f);
        if (chunk == null) return;

        chunk.PlaceBlock(worldPos, normal, type);
    }
}