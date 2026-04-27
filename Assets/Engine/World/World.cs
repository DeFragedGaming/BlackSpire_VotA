using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    public GameObject terrainChunkPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        CreateTerrainChunk(new Vector3Int(0, 0, 0));
    }

    void CreateTerrainChunk(Vector3Int coord)
    {
        if (terrainChunkPrefab == null)
            return;

        GameObject go = Instantiate(terrainChunkPrefab);
        go.transform.position = coord;

        TerrainChunk chunk = go.GetComponent<TerrainChunk>();
        chunk.Init(32, 64, 32);

        TerrainGenerator.Generate(chunk, coord);

        

        MeshFilter mf = go.GetComponent<MeshFilter>();
        MeshCollider mc = go.GetComponent<MeshCollider>();

        
    }
}
