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

    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 moveDirection;

    private float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if(playerCamera == null)
            playerCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();
    }

    void Look()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(
            pitch,
            -89f,
             89f
        );

        playerCamera.localRotation =
            Quaternion.Euler(pitch,0,0);
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input =
            (transform.right * x +
             transform.forward * z).normalized;

        float speed =
            Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : moveSpeed;

        float control =
            controller.isGrounded
            ? acceleration
            : acceleration * airControl;

        Vector3 targetMove =
            input * speed;

        moveDirection = Vector3.Lerp(
            moveDirection,
            targetMove,
            control * Time.deltaTime
        );

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
                velocity.y = jumpForce;
        }

        Vector3 finalMove =
            moveDirection +
            Vector3.up * velocity.y;

        controller.Move(
            finalMove * Time.deltaTime
        );
    }

    void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
    }
}