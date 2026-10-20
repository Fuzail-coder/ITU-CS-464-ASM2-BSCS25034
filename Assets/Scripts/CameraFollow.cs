using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 4, -7);
    public float smoothTime = 0.3f;           
    Vector3 velocity;                         

    void LateUpdate()
    {
        Vector3 wanted = target.position + Quaternion.Euler(0f, target.eulerAngles.y, 0f) * offset;
        transform.position = Vector3.SmoothDamp(transform.position, wanted,
                                                ref velocity, smoothTime);
        transform.LookAt(target);
    }
}