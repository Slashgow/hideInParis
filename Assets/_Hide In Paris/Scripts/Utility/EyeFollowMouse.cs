using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EyeFollowMouse : MonoBehaviour
{
    [Header("Eye Settings")]
    [SerializeField] private Transform eyeCenter; 
    [SerializeField] private float maxDistance = 0.5f; 
    [SerializeField] private float smoothSpeed = 10f; 

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateEye = false; 
    [SerializeField] private float rotationOffset = 0f; 

    private Vector3 originalLocalPosition;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (eyeCenter == null)
            eyeCenter = transform.parent != null ? transform.parent : transform;

        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (mainCamera == null)
            return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = eyeCenter.position.z;

        Vector3 direction = mouseWorldPos - eyeCenter.position;

        float distance = Mathf.Min(direction.magnitude, maxDistance);
        Vector3 targetPosition = eyeCenter.position + direction.normalized * distance;

        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        newPosition.z = transform.position.z; 
        transform.position = newPosition;

        if (rotateEye)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform center = eyeCenter != null ? eyeCenter : transform;

        // Draw the maximum movement radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center.position, maxDistance);

        // Draw line to center
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center.position, transform.position);
    }
}