using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.5f;
    public float sprintSpeed = 7.5f;
    public float acceleration = 14f;
    public float airControl = 0.5f;

    [Header("Jump / Gravity")]
    public float jumpForce = 8f;
    public float gravity = 22f;

    [Header("Mouse")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;

    [Header("Mining")]
    public float mineDistance = 6f;

    [Header("Block System")]
    public GameObject highlightCube;
    public BlockType selectedBlock = BlockType.Grass;

    CharacterController controller;

    Vector3 velocity;
    Vector3 moveDirection;

    float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (highlightCube != null)
            highlightCube.SetActive(false);
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();
        Mine();
        Place();
        HandleHotbar();
        UpdateHighlight();
        VoidCheck();
    }

    
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = (transform.right * x + transform.forward * z).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        float control = controller.isGrounded ? acceleration : acceleration * airControl;

        moveDirection = Vector3.Lerp(moveDirection, input * speed, control * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
                velocity.y = jumpForce;
        }

        Vector3 finalMove = moveDirection + Vector3.up * velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
    }

    
    void Mine()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, mineDistance))
        {
            VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();

            if (chunk != null)
            {
                chunk.RemoveBlock(hit.point, hit.normal);
            }
        }
    }

    
    void Place()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, mineDistance))
        {
            VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();

            if (chunk != null)
            {
                chunk.PlaceBlock(hit.point, hit.normal, selectedBlock);
            }
        }
    }

    
    void HandleHotbar()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedBlock = BlockType.Grass;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedBlock = BlockType.Dirt;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedBlock = BlockType.Stone;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedBlock = BlockType.CoalOre;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedBlock = BlockType.IronOre;
        if (Input.GetKeyDown(KeyCode.Alpha6)) selectedBlock = BlockType.Grass;
        if (Input.GetKeyDown(KeyCode.Alpha7)) selectedBlock = BlockType.Dirt;
        if (Input.GetKeyDown(KeyCode.Alpha8)) selectedBlock = BlockType.Stone;
        if (Input.GetKeyDown(KeyCode.Alpha9)) selectedBlock = BlockType.Stone;
    }

    
    void UpdateHighlight()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, mineDistance))
        {
            if (highlightCube != null)
            {
                highlightCube.SetActive(true);

                Vector3 pos = hit.point + hit.normal * 0.01f;

                highlightCube.transform.position = new Vector3(
                    Mathf.Floor(pos.x) + 0.5f,
                    Mathf.Floor(pos.y) + 0.5f,
                    Mathf.Floor(pos.z) + 0.5f
                );
            }
        }
        else
        {
            if (highlightCube != null)
                highlightCube.SetActive(false);
        }
    }

    
    void VoidCheck()
    {
        if (transform.position.y < -20f)
        {
            transform.position = new Vector3(512, 120, 512);
            velocity = Vector3.zero;
        }
    }
}