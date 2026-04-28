using System.Collections.Generic;
using UnityEngine;

public class BlockData
{
    public byte Id;
    public string Name;

    public bool Solid;
    public bool Transparent;

    public int TopTex;
    public int SideTex;
    public int BottomTex;

    public BlockData(
        byte id,
        string name,
        bool solid,
        bool transparent,
        int topTex,
        int sideTex,
        int bottomTex)
    {
        Id = id;
        Name = name;

        Solid = solid;
        Transparent = transparent;

        TopTex = topTex;
        SideTex = sideTex;
        BottomTex = bottomTex;
    }
}

public static class Blocks
{
    public static readonly Dictionary<byte, BlockData> All =
        new Dictionary<byte, BlockData>()
    {
        {
            (byte)BlockId.Air,
            new BlockData(
                0,
                "Air",
                false,
                true,
                -1,-1,-1
            )
        },

        {
            (byte)BlockId.SurfaceAshGrass,
            new BlockData(
                1,
                "Surface Ash Grass",
                true,
                false,

                0, // top texture
                1, // side texture
                2  // bottom texture
            )
        },

        {
            (byte)BlockId.Dirt,
            new BlockData(
                2,
                "Dirt",
                true,
                false,

                2,2,2
            )
        },

        {
            (byte)BlockId.Stone,
            new BlockData(
                3,
                "Stone",
                true,
                false,

                3,3,3
            )
        },

        {
            (byte)BlockId.Water,
            new BlockData(
                4,
                "Water",
                false,
                true,

                4,4,4
            )
        },

        {
            (byte)BlockId.Blackstone,
            new BlockData(
                5,
                "Blackstone",
                true,
                false,

                5,5,5
            )
        },

        {
            (byte)BlockId.CoalOre,
            new BlockData(
                6,
                "Coal Ore",
                true,
                false,

                6,6,6
            )
        },

        {
            (byte)BlockId.InfernalCrystalOre,
            new BlockData(
                7,
                "Infernal Crystal Ore",
                true,
                false,

                7,7,7
            )
        }
    };
}