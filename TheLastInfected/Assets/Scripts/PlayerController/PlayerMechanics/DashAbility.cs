using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DashAbility : MonoBehaviour
{
    public float dashForce = 10f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;

    public Transform playerBody;

    private CharacterController controller;
    private float lastDashTime = -999f;
    private float lastShiftTime = 0f;

    private WallClimb climbScript;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        climbScript = GetComponent<WallClimb>();
    }

    void Update()
    {
        if (climbScript != null && climbScript.enabled && climbScript.IsClimbing())
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            float timeSinceLastShift = Time.time - lastShiftTime;

            if (timeSinceLastShift <= doubleTapThreshold && Time.time - lastDashTime >= dashCooldown && controller.isGrounded)
            {
                Dash();
                lastDashTime = Time.time;
            }

            lastShiftTime = Time.time;
        }
    }

    void Dash()
    {
        Vector3 dashDirection = Vector3.ProjectOnPlane(playerBody.forward, Vector3.up).normalized;
        controller.Move(dashDirection * dashForce);
    }
}
