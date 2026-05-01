using UnityEngine;

public static class BlockColors
{
    public static Color GetColor(BlockType type)
    {
        switch (type)
        {
            case BlockType.Grass:   return Color.green;
            case BlockType.Dirt:    return new Color(0.5f, 0.25f, 0.1f);
            case BlockType.Stone:   return Color.gray;
            case BlockType.CoalOre: return Color.black;
            case BlockType.IronOre: return new Color(1f, 0.5f, 0.2f);
            case BlockType.Sand:    return new Color(0.9f, 0.85f, 0.6f);
            case BlockType.Water:   return new Color(0.1f, 0.2f, 0.7f, 0.8f);
            case BlockType.Bedrock: return new Color(0.05f, 0.05f, 0.05f);
            default:                return Color.clear;
        }
    }
}
