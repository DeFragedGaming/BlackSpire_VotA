using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    MeshFilter mf;
    MeshCollider mc;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
    }

    public void ApplyMesh(Mesh mesh)
    {
        mf.mesh = mesh;
        StartCoroutine(ApplyCollider(mesh));
    }

    IEnumerator ApplyCollider(Mesh mesh)
    {
        yield return null;
        mc.sharedMesh = null;
        mc.sharedMesh = mesh;
    }
}
