using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;                       // Player ko yahan drag karo
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f); // Camera player ke sar/kandhe ko dekhe

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 1f;

    [Header("Mouse")]
    public float mouseSensitivity = 3f;
    public float minPitch = -30f;                  // neeche dekhne ki limit
    public float maxPitch = 60f;                   // upar dekhne ki limit

    [Header("Smoothing")]
    public float followSmoothTime = 0.05f;

    [Header("Options")]
    public bool rotatePlayerWithCamera = true;     // Player camera ki direction me ghoomay
    public bool lockCursor = true;
    public LayerMask collisionMask = ~0;           // Camera deewar ke andar na jaye
    public float collisionRadius = 0.3f;

    private float yaw;
    private float pitch = 15f;
    private Vector3 currentVelocity;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow: Player assign nahi kiya!");
            enabled = false;
            return;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = player.eulerAngles.y;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        // Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = player.position + targetOffset;

        float finalDistance = distance;
        Vector3 direction = rotation * Vector3.back;

        if (Physics.SphereCast(focusPoint, collisionRadius, direction,
            out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            finalDistance = Mathf.Max(hit.distance, minDistance);
        }

        Vector3 desiredPosition = focusPoint + direction * finalDistance;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition,
            ref currentVelocity, followSmoothTime);
        transform.rotation = rotation;

        if (rotatePlayerWithCamera)
        {
            player.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}