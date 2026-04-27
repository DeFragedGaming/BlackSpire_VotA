using UnityEngine;

public class RuntimeBootstrap : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Bootstrap running");
        if (World.Instance != null)
        {
            Debug.Log("World found — initializing");
            World.Instance.Initialize();
        }
        else
        {
            Debug.LogError("World instance not found in scene.");
        }
    }
}
