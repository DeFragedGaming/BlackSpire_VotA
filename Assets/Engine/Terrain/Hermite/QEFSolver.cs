using UnityEngine;
using System.Collections.Generic;

public static class QEFSolver
{
    public static Vector3 Solve(List<HermiteSample> samples, Vector3 cellCenter)
    {
        if (samples.Count == 0)
            return cellCenter;

        Vector3 sum = Vector3.zero;

        foreach (var s in samples)
            sum += s.position;

        return sum / samples.Count;
    }
}
