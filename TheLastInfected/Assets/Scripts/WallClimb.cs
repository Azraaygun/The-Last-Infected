using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WallClimb : MonoBehaviour
{
    [Header("Climb Settings")]
    public float climbSpeed = 0.7f;
    public float climbDuration = 2.5f;
    public float wallCheckDistance = 1f;
    public float wallCheckRadius = 0.4f;
    public float climbableAngle = 60f;
    public KeyCode climbKey = KeyCode.C;

    [Header("References")]
    public Transform playerCamera;
    public Transform cameraHolder;
    public Animator animator;

    [Header("Camera Lerp")]
    public float cameraLerpSpeed = 5f;
    public float climbCamHeight = 1.4f;
    public float climbCamZOffset = 0f;

    [Header("Mantle Settings")]
    public float mantleCheckHeight = 2f;
    public float mantleOffset = 1.2f;
    public float mantleDuration = 0.4f;

    [Header("Camera Rotation Lock")]
    public float climbCameraXRotation = 3f;

    private CharacterController controller;
    private float climbTimer = 0f;
    private bool isClimbing = false;

    private bool isMantling = false;
    private Vector3 mantleStartPos;
    private Vector3 mantleTargetPos;
    private float mantleTimer = 0f;

    private Vector3 defaultCamLocalPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        defaultCamLocalPos = cameraHolder.localPosition;
    }

    void Update()
    {
        if (isMantling)
        {
            HandleMantleMovement();
            return;
        }

        if (isClimbing && CheckForLedge())
        {
            StartMantle();
            return;
        }

        if (CanClimbNow())
        {
            if (!UIHintManager.Instance.IsHintVisible())
                UIHintManager.Instance.ShowHint("[C] For Wall Climbing");
        }
        else
        {
            if (UIHintManager.Instance.IsHintVisible())
                UIHintManager.Instance.HideHint();
        }

        if (Input.GetKey(climbKey) && IsWallInFront() && climbTimer < climbDuration)
        {
            StartClimb();
        }
        else
        {
            StopClimb();
        }

        LerpCameraPosition();

        if (isClimbing)
        {
            Vector3 camRot = playerCamera.localEulerAngles;
            camRot.x = climbCameraXRotation;
            playerCamera.localEulerAngles = camRot;
        }
    }

    void StartClimb()
    {
        isClimbing = true;
        climbTimer += Time.deltaTime;

        Vector3 move = Vector3.up * climbSpeed;
        controller.Move(move * Time.deltaTime);

        if (animator != null)
            animator.SetBool("isClimbing", true);
    }

    void StopClimb()
    {
        if (!isClimbing) return;

        isClimbing = false;
        climbTimer = 0f;

        if (animator != null)
            animator.SetBool("isClimbing", false);
    }

    void LerpCameraPosition()
    {
        if (cameraHolder == null) return;

        float targetY = isClimbing ? climbCamHeight : defaultCamLocalPos.y;
        float targetZ = isClimbing ? climbCamZOffset : defaultCamLocalPos.z;

        Vector3 currentPos = cameraHolder.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, targetY, targetZ);

        cameraHolder.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * cameraLerpSpeed);
    }

    bool IsWallInFront()
    {
        Vector3 origin = transform.position + Vector3.up * 1.1f;
        Vector3 dir = playerCamera.forward;

        RaycastHit hit;
        if (Physics.SphereCast(origin, wallCheckRadius, dir, out hit, wallCheckDistance))
        {
            float wallAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (wallAngle > climbableAngle)
            {
                float lookDot = Vector3.Dot(playerCamera.forward.normalized, -hit.normal.normalized);
                if (lookDot >= 0.8f)
                    return true;
            }
        }

        return false;
    }

    bool CheckForLedge()
    {
        Vector3 origin = transform.position + Vector3.up * mantleCheckHeight;
        Vector3 dir = playerCamera.forward;

        return !Physics.Raycast(origin, dir, wallCheckDistance);
    }

    void StartMantle()
    {
        isClimbing = false;
        climbTimer = 0f;

        if (animator != null)
            animator.SetBool("isClimbing", false);

        mantleStartPos = transform.position;
        mantleTargetPos = transform.position + Vector3.up * mantleOffset;

        mantleTimer = 0f;
        isMantling = true;
    }

    void HandleMantleMovement()
    {
        mantleTimer += Time.deltaTime;
        float t = mantleTimer / mantleDuration;

        Vector3 newPos = Vector3.Lerp(mantleStartPos, mantleTargetPos, t);

        controller.enabled = false;
        transform.position = newPos;
        controller.enabled = true;

        if (t >= 1f)
        {
            isMantling = false;
        }
    }

    public bool IsClimbing() => isClimbing || isMantling;

    public bool CanClimbNow() => !isClimbing && !isMantling && IsWallInFront();

    void OnDrawGizmos()
    {
        if (playerCamera == null) return;

        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * 1.1f;
        Vector3 dir = playerCamera.forward * wallCheckDistance;
        Gizmos.DrawRay(origin, dir);
        Gizmos.DrawWireSphere(origin + dir, wallCheckRadius);
    }
}