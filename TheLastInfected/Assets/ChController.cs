using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ChController : MonoBehaviour
{
    [Header("Hareket")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float dashForce = 10f;
    public float dashCooldown = 1f;

    [Header("Mouse")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private float lastDashTime = -999f;
    private float lastShiftTime = 0f;
    private float doubleTapThreshold = 0.3f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        float currentSpeed = walkSpeed;

        if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            float timeSinceLastShift = Time.time - lastShiftTime;
            if (timeSinceLastShift <= doubleTapThreshold && Time.time - lastDashTime >= dashCooldown && isGrounded)
            {
                Dash();
                lastDashTime = Time.time;
            }
            lastShiftTime = Time.time;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Dash()
    {
        Vector3 dashDirection = playerBody.forward;
        controller.Move(dashDirection.normalized * dashForce);
    }
}
