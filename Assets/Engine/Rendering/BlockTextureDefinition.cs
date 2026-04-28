using UnityEngine;

[CreateAssetMenu(menuName="BlackSpire/Block Texture")]
public class BlockTextureDefinition : ScriptableObject
{
    public int blockID;
    public string blockName;

    public Texture2D top;
    public Texture2D side;
    public Texture2D bottom;

    public bool sameTextureAllSides;
}