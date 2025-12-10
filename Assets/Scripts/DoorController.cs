using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    
    [Header("Key Requirements")]
    [SerializeField] private bool requiresKeys = false;
    [SerializeField] private int keysRequired = 3;
    
    private bool isOpen = false;
    private bool playerInZone = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start() {
        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update() {
        bool canOpen = playerInZone;
        
        if (requiresKeys) {
            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
            if (inventory != null) {
                canOpen = canOpen && inventory.KeyCount >= keysRequired;
            } else {
                canOpen = false;
            }
        }
        
        if (canOpen && !isOpen) {
            isOpen = true;
        }
        
        if (isOpen) {
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, openRotation, openSpeed * Time.deltaTime);
        } else {
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, closedRotation, openSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInZone = false;
            if (!requiresKeys) {
                isOpen = false;
            }
        }
    }
}