using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour {
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Camera Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;
    
    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    
    private float currentX = 0f;
    private float currentY = 20f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        if (target == null) return;
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        currentX += mouseDelta.x * mouseSensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
        
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back;
        Vector3 targetPos = target.position + Vector3.up * height;
        Vector3 desiredPosition = targetPos + direction * distance;
        
        RaycastHit hit;
        float currentDistance = distance;
        
        if (Physics.SphereCast(targetPos, collisionRadius, direction, out hit, distance, collisionLayers)) {
            currentDistance = hit.distance - collisionRadius;
            currentDistance = Mathf.Max(currentDistance, 0.5f);
        }
        
        desiredPosition = targetPos + direction * currentDistance;
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        transform.LookAt(targetPos);
    }
    
    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}