using UnityEngine;
using System.Collections.Generic;

public class BlackSpireWorld : MonoBehaviour
{
    [Header("World")]
    public int worldSeed = 1337;
    public int chunkSize = 16;
    public int chunkHeight = 128;
    public int viewDistance = 4;

    public Transform player;
    public Material voxelMaterial;

    Dictionary<Vector2Int,VoxelChunk> chunks =
        new Dictionary<Vector2Int,VoxelChunk>();

    void Start()
    {
        Random.InitState(worldSeed);
        GenerateInitialWorld();
    }

    void Update()
    {
        UpdateChunks();
    }

    void GenerateInitialWorld()
    {
        for(int x=-viewDistance;x<=viewDistance;x++)
        for(int z=-viewDistance;z<=viewDistance;z++)
        {
            CreateChunk(x,z);
        }
    }

    void UpdateChunks()
    {
        int px = Mathf.FloorToInt(
            player.position.x/chunkSize);

        int pz = Mathf.FloorToInt(
            player.position.z/chunkSize);

        for(int x=px-viewDistance;x<=px+viewDistance;x++)
        for(int z=pz-viewDistance;z<=pz+viewDistance;z++)
        {
            Vector2Int c = new Vector2Int(x,z);

            if(!chunks.ContainsKey(c))
                CreateChunk(x,z);
        }
    }

    void CreateChunk(int cx,int cz)
    {
        GameObject go = new GameObject(
            $"Chunk_{cx}_{cz}");

        go.transform.parent=transform;
        go.transform.position=
            new Vector3(
                cx*chunkSize,
                0,
                cz*chunkSize);

        go.AddComponent<MeshRenderer>().material=
            voxelMaterial;

        VoxelChunk chunk=
            go.AddComponent<VoxelChunk>();

        chunk.sizeX=chunkSize;
        chunk.sizeY=chunkHeight;
        chunk.sizeZ=chunkSize;

        chunk.worldX=cx;
        chunk.worldZ=cz;

        GenerateTerrain(chunk);

        chunk.BuildMesh();

        chunks.Add(
            new Vector2Int(cx,cz),
            chunk);
    }

    void GenerateTerrain(VoxelChunk chunk)
    {
        for(int x=0;x<chunk.sizeX;x++)
        for(int z=0;z<chunk.sizeZ;z++)
        {
            int wx =
              x + chunk.worldX*chunkSize;

            int wz =
              z + chunk.worldZ*chunkSize;

            float continent=
                FractalNoise(
                    wx*.0025f,
                    wz*.0025f,
                    4,
                    .5f);

            float mountains=
                RidgeNoise(
                    wx*.008f,
                    wz*.008f);

            float hills=
                Mathf.PerlinNoise(
                    wx*.03f,
                    wz*.03f)*8f;

            int surface=
                Mathf.FloorToInt(
                    40+
                    continent*30+
                    mountains*35+
                    hills);

            for(int y=0;y<chunk.sizeY;y++)
            {
                bool solid=
                    y<=surface;

                if(solid)
                {
                    float cave=
                      CaveNoise(wx,y,wz);

                    if(cave>.62f)
                        solid=false;
                }

                if(y<20)
                {
                    float abyss=
                      CaveNoise(
                       wx*1.5f,
                       y*1.2f,
                       wz*1.5f);

                    if(abyss<.40f)
                        solid=true;
                }

                if(!solid)
                {
                    if(y<28)
                    {
                        chunk.SetBlock(
                          x,y,z,
                          4);
                    }

                    continue;
                }

                if(y==surface)
                {
                    chunk.SetBlock(x,y,z,1);
                }
                else if(y>surface-4)
                {
                    chunk.SetBlock(x,y,z,2);
                }
                else if(y<20)
                {
                    chunk.SetBlock(x,y,z,5);
                }
                else
                {
                    chunk.SetBlock(x,y,z,3);
                }

                GenerateOres(
                    chunk,
                    x,y,z,
                    wx,wz);
            }
        }
    }

    void GenerateOres(
        VoxelChunk chunk,
        int x,int y,int z,
        int wx,int wz)
    {
        float ore=
            Mathf.PerlinNoise(
                wx*.08f+y*.04f,
                wz*.08f);

        if(y<35 && ore>.78f)
            chunk.SetBlock(x,y,z,6);

        if(y<15 && ore>.87f)
            chunk.SetBlock(x,y,z,7);
    }

    float FractalNoise(
        float x,
        float z,
        int oct,
        float persistence)
    {
        float total=0;
        float amp=1;
        float freq=1;
        float max=0;

        for(int i=0;i<oct;i++)
        {
            total +=
                Mathf.PerlinNoise(
                    x*freq,
                    z*freq)*amp;

            max+=amp;
            amp*=persistence;
            freq*=2;
        }

        return total/max;
    }

    float RidgeNoise(float x,float z)
    {
        float n=
            Mathf.PerlinNoise(x,z);

        n=1f-Mathf.Abs(2*n-1);
        return n*n;
    }

    float CaveNoise(
        float x,
        float y,
        float z)
    {
        float a=
            Mathf.PerlinNoise(
                x*.05f+y*.03f,
                z*.05f);

        float b=
            Mathf.PerlinNoise(
                z*.05f+y*.03f,
                x*.05f);

        return (a+b)*.5f;
    }
}