using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 1f;
    
    private Animator animator;
    private Vector3 movement;
    private Vector2 moveInput;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        moveInput = new Vector2(
            Keyboard.current.dKey.isPressed ? 1 : (Keyboard.current.aKey.isPressed ? -1 : 0),
            Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0)
        );
        
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        movement = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        
        bool isWalking = movement.magnitude > 0;
        animator.SetBool("isWalking", isWalking);
        
        if (movement.magnitude > 0) {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720f * Time.deltaTime);
        }
    }
    
    void OnAnimatorMove() {
        Vector3 velocity = animator.deltaPosition * moveSpeed / Time.deltaTime;
        velocity.y = 0;
        
        transform.position += velocity * Time.deltaTime;
    }
}