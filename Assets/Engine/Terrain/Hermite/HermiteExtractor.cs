using UnityEngine;

public static class HermiteExtractor
{
    public static bool TrySampleEdge(
        TerrainDensityField field,
        Vector3Int p0,
        Vector3Int p1,
        out HermiteSample sample)
    {
        float d0 = field.Get(p0.x, p0.y, p0.z);
        float d1 = field.Get(p1.x, p1.y, p1.z);

        if ((d0 > 0 && d1 > 0) || (d0 < 0 && d1 < 0))
        {
            sample = default;
            return false;
        }

        float t = d0 / (d0 - d1);
        Vector3 pos = Vector3.Lerp(p0, p1, t);

        Vector3 normal = EstimateNormal(field, pos);

        sample = new HermiteSample(pos, normal);
        return true;
    }

    static Vector3 EstimateNormal(TerrainDensityField field, Vector3 pos)
    {
        float dx = Sample(field, pos + Vector3.right) - Sample(field, pos - Vector3.right);
        float dy = Sample(field, pos + Vector3.up) - Sample(field, pos - Vector3.up);
        float dz = Sample(field, pos + Vector3.forward) - Sample(field, pos - Vector3.forward);

        return new Vector3(dx, dy, dz).normalized;
    }

    static float Sample(TerrainDensityField field, Vector3 pos)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(pos.x), 0, field.sizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(pos.y), 0, field.sizeY - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(pos.z), 0, field.sizeZ - 1);

        return field.Get(x, y, z);
    }
}
