using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform fpsCameraPosition;
    
    [Header("Camera Mode")]
    [SerializeField] private bool isFPS = false;
    
    [Header("TPS Settings")]
    [SerializeField] private float tpsDistance = 5f;
    [SerializeField] private float tpsHeight = 2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("FPS Settings")]
    [SerializeField] private float fpsMouseSensitivity = 100f;
    
    [Header("TPS Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;
    
    [Header("FPS Rotation Limits")]
    [SerializeField] private float fpsMinVerticalAngle = -80f;
    [SerializeField] private float fpsMaxVerticalAngle = 80f;
    
    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    
    private float currentX = 0f;
    private float currentY = 20f;
    private float fpsRotationX = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (fpsCameraPosition == null && target != null) {
            GameObject fpsPos = new GameObject("FPS_CameraPosition");
            fpsPos.transform.SetParent(target);
            fpsPos.transform.localPosition = new Vector3(0, 1.3f, 0.2f);
            fpsPos.transform.localRotation = Quaternion.identity;
            fpsCameraPosition = fpsPos.transform;
        }
    }

    void Update() {
        if (Keyboard.current.cKey.wasPressedThisFrame) {
            isFPS = !isFPS;
            
            if (isFPS) {
                fpsRotationX = currentY;
                if (target != null) {
                    target.rotation = Quaternion.Euler(0, currentX, 0);
                }
            } else {
                currentY = fpsRotationX;
            }
        }
    }

    void LateUpdate() {
        if (target == null) return;
        
        if (isFPS) {
            UpdateFPSCamera();
        } else {
            UpdateTPSCamera();
        }
    }

    void UpdateTPSCamera() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        currentX += mouseDelta.x * mouseSensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
        
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back;
        Vector3 targetPos = target.position + Vector3.up * tpsHeight;
        Vector3 desiredPosition = targetPos + direction * tpsDistance;
        
        RaycastHit hit;
        float currentDistance = tpsDistance;
        
        if (Physics.SphereCast(targetPos, collisionRadius, direction, out hit, tpsDistance, collisionLayers)) {
            currentDistance = hit.distance - collisionRadius;
            currentDistance = Mathf.Max(currentDistance, 0.5f);
        }
        
        desiredPosition = targetPos + direction * currentDistance;
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(targetPos);
    }

    void UpdateFPSCamera() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        fpsRotationX -= mouseDelta.y * fpsMouseSensitivity * Time.deltaTime;
        fpsRotationX = Mathf.Clamp(fpsRotationX, fpsMinVerticalAngle, fpsMaxVerticalAngle);
        
        float mouseX = mouseDelta.x * fpsMouseSensitivity * Time.deltaTime;
        target.Rotate(Vector3.up * mouseX);
        
        transform.position = fpsCameraPosition.position;
        transform.rotation = Quaternion.Euler(fpsRotationX, target.eulerAngles.y, 0);
    }
    
    void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public bool IsFPSMode() {
        return isFPS;
    }
    
    public float GetCameraYRotation() {
        return isFPS ? target.eulerAngles.y : currentX;
    }
}