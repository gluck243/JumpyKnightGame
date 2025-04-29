using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Drag player object here
    [SerializeField] private float smoothTime = 0.3f; // Smoothing time
    [SerializeField] private Vector3 offset; // Camera offset
    [SerializeField] private float cameraZDepth = -10f; // Camera Z depth

    private Vector3 currentVelocity; // Used for smoothDamp

    // Update is called once per frame
    void LateUpdate() // LateUpdate is good for camera follow - runs after Update()
    {
        if (target == null) return; //if no target assigned, do nothing
        Vector3 targetPosition = target.position + offset; // Calculate target position
        // Smoothly move the camera towards the target position in X and Y
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // Force the camera's Z position to be a fixed value
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, cameraZDepth);
    }
}
