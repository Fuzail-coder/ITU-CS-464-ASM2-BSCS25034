using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 6f;
    [SerializeField] float fallMultiplier = 2.2f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBuffer = 0.12f;
    [SerializeField] Transform cam;

    [SerializeField] Transform feet;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] LayerMask groundMask;

    Rigidbody rb;
    bool isGrounded, jumpQueued;
    float lastGroundedTime = -1f, lastPressedTime = -1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (cam == null) cam = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = IsGrounded();

        if (isGrounded && !jumpQueued && rb.linearVelocity.y <= 0f)
            lastGroundedTime = Time.time;
        if (Input.GetKeyDown(KeyCode.Space))
            lastPressedTime = Time.time;

        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        bool wantJump = Time.time - lastPressedTime <= jumpBuffer;
        if (canJump && wantJump)
        {
            jumpQueued = true;
            lastGroundedTime = lastPressedTime = -1f;  
        }
        Debug.Log($"grounded={isGrounded} velY={rb.linearVelocity.y:F2} queued={jumpQueued}");
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;
        move = Vector3.ClampMagnitude(move, 1f) * speed;

        rb.linearVelocity = new Vector3(
            move.x, rb.linearVelocity.y, move.z);

        if (jumpQueued)
        {
            Vector3 vel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpQueued = false;
        }

        if (rb.linearVelocity.y < 0f)   
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f),
                        ForceMode.Acceleration);
    }

    bool IsGrounded() =>
        Physics.CheckSphere(feet.position, checkRadius, groundMask);

    void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, checkRadius);
    }
}