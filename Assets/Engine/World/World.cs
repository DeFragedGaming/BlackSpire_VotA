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
        Debug.Log("World initialized — creating terrain chunk");

        CreateTerrainChunk(new Vector3Int(0, 0, 0));
    }

    void CreateTerrainChunk(Vector3Int coord)
    {
        if (terrainChunkPrefab == null)
        {
            Debug.LogError("TerrainChunk prefab is NOT assigned!");
            return;
        }

        GameObject go = Instantiate(terrainChunkPrefab);
        go.transform.position = coord;

        TerrainChunk chunk = go.GetComponent<TerrainChunk>();
        chunk.Init(32, 64, 32);

        TerrainGenerator.Generate(chunk, coord);

        Mesh mesh = DualContourMesher.BuildMesh(chunk);

        if (mesh.vertexCount == 0)
        {
            Debug.LogWarning("Generated mesh has NO vertices — density field may be wrong.");
        }

        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
