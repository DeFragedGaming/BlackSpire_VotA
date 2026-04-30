using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform playerBody;
    public float distance = 1.6f;
    public float minDistance = 0.3f;
    public float smoothSpeed = 10f;
    public LayerMask collisionMask;
    public Transform playerCamera;
    Vector3 currentVelocity;

    void LateUpdate()
{
    PreventClipping();
}

void PreventClipping()
{
    if (playerCamera == null) return;

    // Define head position properly
    Vector3 headPos = transform.position + Vector3.up * 1.6f;

    Ray ray = new Ray(headPos, playerCamera.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
    {
        playerCamera.localPosition = new Vector3(0, 1.6f, -0.1f);
    }
    else
    {
        playerCamera.localPosition = new Vector3(0, 1.6f, 0f);
    }
    }
}