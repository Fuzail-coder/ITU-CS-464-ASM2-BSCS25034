using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    public float jumpForce = 6f, fallMultiplier = 2.2f;
    public float coyoteTime = 0.1f, jumpBuffer = 0.12f;
    public float turnSpeed = 180f;            // degrees per second (Q / E)
    Rigidbody rb;
    bool isGrounded, jumpQueued;
    float lastGroundedTime = -1f, lastPressedTime = -1f;
    float yaw;

    [SerializeField] Transform feet;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] LayerMask groundMask;

    // ---------- Unity lifecycle ----------
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // freeze only X and Z so MoveRotation can still turn the player on Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        yaw = rb.rotation.eulerAngles.y;
    }

    void Update()
    {
        // Q turns left, E turns right
        float turn = 0f;
        if (Input.GetKey(KeyCode.E)) turn += 1f;
        if (Input.GetKey(KeyCode.Q)) turn -= 1f;
        yaw += turn * turnSpeed * Time.deltaTime;

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
    }

    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        move = Vector3.ClampMagnitude(move, 1f) * speed;
        move = Quaternion.Euler(0f, yaw, 0f) * move;   // WASD relative to where the player faces
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

    // ---------- Collision callbacks ----------
    void OnCollisionStay(Collision c)
    {
        if (c.GetContact(0).normal.y > 0.5f)
            isGrounded = true;
    }

    void OnCollisionExit(Collision c) => isGrounded = false;

    // ---------- Ground check + gizmo ----------
    bool IsGrounded() =>
        Physics.CheckSphere(feet.position, checkRadius, groundMask);

    void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, checkRadius);
    }
}