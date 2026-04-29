using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    public GameObject chunkPrefab;
    public Transform player;

    [Header("World Settings")]
    public int biomeSizeChunks = 32;
    public int viewDistance = 6;

    Dictionary<Vector2Int, GameObject> chunks =
        new Dictionary<Vector2Int, GameObject>();

    Vector2Int playerChunk;
    bool hasSpawnedPlayer = false;

    void Start()
    {
        if (chunkPrefab == null || player == null)
        {
            Debug.LogError("Missing references.");
            return;
        }

        playerChunk = GetChunkCoord();

        UpdateVisibleChunks(playerChunk);

        SpawnPlayerOnTerrain();
    }

    void Update()
    {
        Vector2Int newChunk = GetChunkCoord();

        if (newChunk != playerChunk)
        {
            playerChunk = newChunk;
            UpdateVisibleChunks(playerChunk);
        }
    }

    Vector2Int GetChunkCoord()
    {
        return new Vector2Int(
            Mathf.FloorToInt(player.position.x / VoxelData.ChunkWidth),
            Mathf.FloorToInt(player.position.z / VoxelData.ChunkWidth)
        );
    }

    void UpdateVisibleChunks(Vector2Int center)
    {
        HashSet<Vector2Int> needed = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            Vector2Int coord = new Vector2Int(
                center.x + x,
                center.y + z
            );

            if (coord.x < 0 || coord.y < 0 ||
                coord.x >= biomeSizeChunks ||
                coord.y >= biomeSizeChunks)
                continue;

            needed.Add(coord);

            if (!chunks.ContainsKey(coord))
                CreateChunk(coord);
        }

        List<Vector2Int> remove = new List<Vector2Int>();

        foreach (var kv in chunks)
        {
            if (!needed.Contains(kv.Key))
            {
                Destroy(kv.Value);
                remove.Add(kv.Key);
            }
        }

        foreach (var c in remove)
            chunks.Remove(c);
    }

    void CreateChunk(Vector2Int coord)
    {
        GameObject chunk = Instantiate(
            chunkPrefab,
            new Vector3(
                coord.x * VoxelData.ChunkWidth,
                0,
                coord.y * VoxelData.ChunkWidth
            ),
            Quaternion.identity,
            transform
        );

        VoxelChunk vc = chunk.GetComponent<VoxelChunk>();
        vc.Init(coord);

        chunks.Add(coord, chunk);
    }

    void SpawnPlayerOnTerrain()
    {
        if (hasSpawnedPlayer) return;

        int centerChunk = biomeSizeChunks / 2;

        float x = centerChunk * VoxelData.ChunkWidth + 8;
        float z = centerChunk * VoxelData.ChunkWidth + 8;

        float height = SampleTerrainHeight(x, z);

        player.position = new Vector3(x, height + 5f, z);

        hasSpawnedPlayer = true;

        Debug.Log("Player spawned on terrain safely.");
    }

    float SampleTerrainHeight(float x, float z)
    {
        float continent = Mathf.PerlinNoise(x * .01f, z * .01f) * 30f;
        float hills = Mathf.PerlinNoise(x * .04f, z * .04f) * 10f;

        return 30f + continent + hills;
    }
}