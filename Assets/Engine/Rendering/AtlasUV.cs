using UnityEngine;

public static class AtlasUV
{
    public static Vector2[] GetFaceUVs(
        int blockID,
        int face,
        BlockAtlasDatabase db)
    {
        BlockUVData b=null;

        foreach(var x in db.blocks)
        {
            if(x.blockID==blockID)
            {
                b=x;
                break;
            }
        }

        Rect r=
         (face==3)
            ? b.top
         : (face==2)
            ? b.bottom
            : b.side;

        return new Vector2[]
        {
            new Vector2(r.xMin,r.yMin),
            new Vector2(r.xMin,r.yMax),
            new Vector2(r.xMax,r.yMax),
            new Vector2(r.xMax,r.yMin)
        };
    }
}