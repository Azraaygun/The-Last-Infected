using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ChController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float standHeight = 1.3f;
    [SerializeField] private float crouchHeight = 0.7f;
    [SerializeField] private float cameraLerpSpeed = 5f;

    [SerializeField] private Animator animator;

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

    private bool isRunning;
    private bool hasJumped;

    private WallClimb climbScript;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        climbScript = GetComponent<WallClimb>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        UpdateAnimations();
    }

    void HandleMouseLook()
    {
        if (climbScript != null && climbScript.enabled && climbScript.IsClimbing())
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        if (climbScript != null && climbScript.enabled && climbScript.IsClimbing())
            return;

        isGrounded = controller.isGrounded;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float inputMagnitude = new Vector2(moveX, moveZ).magnitude;

        isRunning = Input.GetKey(KeyCode.LeftShift) && isGrounded;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 forward = Vector3.ProjectOnPlane(playerBody.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(playerBody.right, Vector3.up).normalized;
        Vector3 move = right * moveX + forward * moveZ;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasJumped)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            hasJumped = true;
            animator.SetTrigger("Jump");
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            hasJumped = false;
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
        Vector3 dashDirection = Vector3.ProjectOnPlane(playerBody.forward, Vector3.up).normalized;
        controller.Move(dashDirection * dashForce);
    }

    void UpdateAnimations()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float inputMagnitude = new Vector2(moveX, moveZ).magnitude;

        animator.SetFloat("MoveSpeed", inputMagnitude);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);

        float targetHeight = standHeight;

        if (isRunning)
            targetHeight = crouchHeight;

        if (climbScript != null && climbScript.enabled && climbScript.IsClimbing())
            return; 

        Vector3 camPos = cameraHolder.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetHeight, Time.deltaTime * cameraLerpSpeed);
        cameraHolder.localPosition = camPos;
    }
}
