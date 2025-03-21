using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public CharacterController controller;

    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 lastPosition;

    public float headBobFrequency = 10f;
    public float headBobAmplitude = 0.05f;
    private float headBobTimer = 0f;
    private Vector3 initialCameraPosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraPosition = playerCamera.localPosition;
        lastPosition = transform.position;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        Move();
        LookAround();
        ApplyGravity();
        HeadBobbing();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // A, D o frecce sinistra/destra
        float moveZ = Input.GetAxis("Vertical");   // W, S o frecce su/giù

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HeadBobbing()
    {
        Vector3 movement = transform.position - lastPosition;
        float speed = movement.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (speed > 0.1f && isGrounded)
        {
            headBobTimer += Time.deltaTime * speed * headBobFrequency;
            float bobOffset = Mathf.Sin(headBobTimer) * headBobAmplitude;
            playerCamera.localPosition = initialCameraPosition + new Vector3(0, bobOffset, 0);
        }
        else
        {
            headBobTimer = 0f;
            playerCamera.localPosition = initialCameraPosition;
        }
    }
}


