using UnityEngine;

public class BlockHighlight : MonoBehaviour
{
    public Transform playerCamera;
    public float distance = 6f;

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Vector3 pos = hit.point - hit.normal * 0.01f;

            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}