using UnityEngine;
using UnityEditor;

public class VoxelHighlightGenerator : EditorWindow
{
    [MenuItem("Tools/Voxel/Generate Highlight System")]
    public static void Generate()
    {
        // ----------------------------
        // CREATE HIGHLIGHT CUBE
        // ----------------------------
        GameObject highlight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlight.name = "BlockHighlight";

        // IMPORTANT: remove collider
        Object.DestroyImmediate(highlight.GetComponent<Collider>());

        // perfect voxel scale (no overlap like your screenshot)
        highlight.transform.localScale = Vector3.one * 1.001f;

        // ----------------------------
        // SAFE SHADER (NO URP DEPENDENCY)
        // ----------------------------
        Shader shader =
            Shader.Find("Standard") ??
            Shader.Find("Universal Render Pipeline/Lit") ??
            Shader.Find("Legacy Shaders/Diffuse");

        if (shader == null)
        {
            Debug.LogError("No valid shader found.");
            return;
        }

        Material mat = new Material(shader);
        mat.color = new Color(0f, 1f, 1f, 0.35f);

        // transparency setup
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        highlight.GetComponent<MeshRenderer>().sharedMaterial = mat;

        // start above ground so it's visible instantly
        highlight.transform.position = new Vector3(0, 2, 0);

        // ----------------------------
        // CONTROLLER OBJECT
        // ----------------------------
        GameObject controller = new GameObject("VoxelHighlighterController");

        BlockHighlighterAuto script = controller.AddComponent<BlockHighlighterAuto>();
        script.highlightBox = highlight.transform;

        if (Camera.main != null)
            script.playerCamera = Camera.main;

        Selection.activeGameObject = controller;

        Debug.Log("Voxel Highlight System created successfully.");
    }
}