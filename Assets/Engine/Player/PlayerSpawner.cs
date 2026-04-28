using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        Vector3 startPos = new Vector3(50, 200, 50);

        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 500))
        {
            player.position = hit.point + Vector3.up * 2f;
        }
    }
}