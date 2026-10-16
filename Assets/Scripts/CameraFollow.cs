using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 4, -7);
    public float smoothTime = 0.3f;           // about the time to catch up
    public float lookAhead = 0.35f;           // seconds of velocity to aim ahead

    Vector3 velocity;                         // SmoothDamp's memory
    Rigidbody targetRb;

    void Awake()
    {
        if (target == null)
            target = FindAnyObjectByType<PlayerController>().transform;
        targetRb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        Vector3 flatVel = targetRb.linearVelocity;
        flatVel.y = 0f;
        Vector3 aim = target.position + flatVel * lookAhead;

        Vector3 wanted = aim + offset;
        transform.position = Vector3.SmoothDamp(transform.position, wanted,
                                                ref velocity, smoothTime);
        transform.LookAt(aim);                // look at the aim point, not the player
    }
}