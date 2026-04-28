using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="BlackSpire/Atlas Database")]
public class BlockAtlasDatabase : ScriptableObject
{
    public Texture2D atlas;

    public List<BlockUVData> blocks=
        new List<BlockUVData>();
}

[System.Serializable]
public class BlockUVData
{
    public int blockID;

    public Rect top;
    public Rect side;
    public Rect bottom;
}