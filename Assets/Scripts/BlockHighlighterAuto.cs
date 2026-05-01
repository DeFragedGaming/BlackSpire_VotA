using UnityEngine;

public class BlockHighlighterAuto : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform highlightBox;

    [Header("Settings")]
    public float maxDistance = 6f;

    private Vector3 lastBlockPos;

    void Update()
    {
        if (playerCamera == null || highlightBox == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            
            Vector3Int block = Vector3Int.FloorToInt(hit.point - hit.normal * 0.01f);

            Vector3 targetPos = block + new Vector3(0.5f, 0.5f, 0.5f);

            
            if (targetPos != lastBlockPos)
            {
                lastBlockPos = targetPos;
                highlightBox.position = targetPos;
            }

            if (!highlightBox.gameObject.activeSelf)
                highlightBox.gameObject.SetActive(true);
        }
        else
        {
            if (highlightBox.gameObject.activeSelf)
                highlightBox.gameObject.SetActive(false);
        }
    }
}