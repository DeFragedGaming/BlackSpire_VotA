using UnityEngine;

public class BlockHighlighter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform highlightBox;

    [Header("Settings")]
    public float maxDistance = 6f;
    public LayerMask blockMask;

    private Vector3 lastBlockPos;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, blockMask))
        {
           
            Vector3 pos = hit.point - hit.normal * 0.01f;

            
            Vector3 blockPos = new Vector3(
                Mathf.Floor(pos.x + 0.5f),
                Mathf.Floor(pos.y + 0.5f),
                Mathf.Floor(pos.z + 0.5f)
            );

            
            if (blockPos != lastBlockPos)
            {
                lastBlockPos = blockPos;

               
                highlightBox.position = blockPos;
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