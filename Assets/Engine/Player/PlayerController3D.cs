using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed=5.5f;
    public float sprintSpeed=7.5f;
    public float acceleration=14f;
    public float airControl=.5f;

    [Header("Jump/Gravity")]
    public float jumpForce=8f;
    public float gravity=22f;

    [Header("Mouse")]
    public Transform playerCamera;
    public float mouseSensitivity=2f;

    [Header("Mining Placeholder")]
    public float mineDistance=6f;

    CharacterController controller;

    Vector3 velocity;
    Vector3 moveDirection;

    float pitch;

    void Start()
    {
        controller=
            GetComponent<CharacterController>();

        if(playerCamera==null)
            playerCamera=
                Camera.main.transform;

        Cursor.lockState=
            CursorLockMode.Locked;
        Cursor.visible=false;
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();
        MiningPlaceholder();
        VoidRescue();
    }

    void Look()
    {
        float mx=
            Input.GetAxis("Mouse X")
            *mouseSensitivity;

        float my=
            Input.GetAxis("Mouse Y")
            *mouseSensitivity;

        transform.Rotate(
            Vector3.up*mx
        );

        pitch-=my;
        pitch=Mathf.Clamp(
            pitch,-89,89
        );

        playerCamera.localRotation=
            Quaternion.Euler(
                pitch,0,0
            );
    }

    void Move()
    {
        float x=
            Input.GetAxisRaw(
                "Horizontal");

        float z=
            Input.GetAxisRaw(
                "Vertical");

        Vector3 input=
            (
             transform.right*x+
             transform.forward*z
            ).normalized;

        float speed=
            Input.GetKey(
                KeyCode.LeftShift)
                ?
                sprintSpeed:
                moveSpeed;

        float control=
            controller.isGrounded
            ?
            acceleration
            :
            acceleration*airControl;

        Vector3 target=
            input*speed;

        moveDirection=
            Vector3.Lerp(
                moveDirection,
                target,
                control*
                Time.deltaTime
            );

        if(controller.isGrounded)
        {
            if(velocity.y<0)
                velocity.y=-2;

            if(Input.GetButtonDown(
                "Jump"))
                velocity.y=
                    jumpForce;
        }

        Vector3 finalMove=
            moveDirection+
            Vector3.up*
            velocity.y;

        controller.Move(
            finalMove*
            Time.deltaTime
        );
    }

    void ApplyGravity()
    {
        velocity.y-=
            gravity*
            Time.deltaTime;
    }

    void VoidRescue()
    {
        if(transform.position.y<-20)
        {
            transform.position=
                new Vector3(
                    512,100,512
                );
        }
    }

    void MiningPlaceholder()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray=
                new Ray(
                    playerCamera.position,
                    playerCamera.forward
                );

            if(Physics.Raycast(
                ray,
                out RaycastHit hit,
                mineDistance))
            {
                Debug.Log(
                 "Mining hit "+
                  hit.point
                );
            }
        }
    }
}