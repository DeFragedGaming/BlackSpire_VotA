using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class BlackSpireAtlasGenerator : EditorWindow
{
    List<BlockTextureDefinition> blocks=
        new List<BlockTextureDefinition>();

    int tileSize=32;
    int padding=2;
    int atlasSize=2048;

    [MenuItem("BlackSpire/Atlas Generator")]
    static void Open()
    {
        GetWindow<BlackSpireAtlasGenerator>();
    }

    void OnGUI()
    {
        GUILayout.Label(
            "BlackSpire Atlas Generator",
            EditorStyles.boldLabel);

        tileSize=
            EditorGUILayout.IntField(
                "Tile Size",
                tileSize);

        padding=
            EditorGUILayout.IntField(
                "Padding",
                padding);

        atlasSize=
            EditorGUILayout.IntField(
                "Atlas Size",
                atlasSize);

        if(GUILayout.Button("Load Block Assets"))
            LoadBlocks();

        GUILayout.Space(10);

        GUILayout.Label(
            "Loaded Blocks: "+blocks.Count);

        if(GUILayout.Button("Generate Atlas"))
            GenerateAtlas();
    }

    void LoadBlocks()
    {
        blocks.Clear();

        string[] guids=
            AssetDatabase.FindAssets(
                "t:BlockTextureDefinition");

        foreach(string g in guids)
        {
            string path=
                AssetDatabase.GUIDToAssetPath(g);

            var block=
             AssetDatabase.LoadAssetAtPath
             <BlockTextureDefinition>(path);

            blocks.Add(block);
        }
    }

    void GenerateAtlas()
    {
        List<Texture2D> textures=
            new List<Texture2D>();

        Dictionary<string,int> indexMap=
            new Dictionary<string,int>();

        foreach(var b in blocks)
        {
            AddUniqueTexture(
                b.top,
                textures,
                indexMap);

            AddUniqueTexture(
                b.side,
                textures,
                indexMap);

            AddUniqueTexture(
                b.bottom,
                textures,
                indexMap);
        }

        Texture2D atlas=
            new Texture2D(
                atlasSize,
                atlasSize);

        Rect[] rects=
            atlas.PackTextures(
                textures.ToArray(),
                padding,
                atlasSize);

        byte[] png=
            atlas.EncodeToPNG();

        string atlasPath=
         "Assets/Generated/BlackSpireAtlas.png";

        Directory.CreateDirectory(
         "Assets/Generated");

        File.WriteAllBytes(
            atlasPath,
            png);

        AssetDatabase.Refresh();

        Texture2D atlasTex=
         AssetDatabase.LoadAssetAtPath
         <Texture2D>(atlasPath);

        BlockAtlasDatabase db=
            ScriptableObject.CreateInstance
            <BlockAtlasDatabase>();

        db.atlas=atlasTex;

        foreach(var b in blocks)
        {
            BlockUVData uv=
                new BlockUVData();

            uv.blockID=b.blockID;

            uv.top=
              rects[
                indexMap[b.top.name]
              ];

            uv.side=
              rects[
                indexMap[b.side.name]
              ];

            uv.bottom=
              rects[
                indexMap[b.bottom.name]
              ];

            db.blocks.Add(uv);
        }

        AssetDatabase.CreateAsset(
            db,
            "Assets/Generated/BlockAtlasDatabase.asset"
        );

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Done",
            "Atlas Generated",
            "OK");
    }

    void AddUniqueTexture(
        Texture2D tex,
        List<Texture2D> textures,
        Dictionary<string,int> map)
    {
        if(tex==null)
            return;

        if(map.ContainsKey(tex.name))
            return;

        map.Add(
            tex.name,
            textures.Count);

        textures.Add(tex);
    }
}