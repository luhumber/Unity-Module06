using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f; // Multiplicateur de vitesse
    
    private Animator animator;
    private Vector3 movement;
    private Vector2 moveInput;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = new Vector2(
            Keyboard.current.dKey.isPressed ? 1 : (Keyboard.current.aKey.isPressed ? -1 : 0),
            Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0)
        );
        
        movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        
        bool isWalking = movement.magnitude > 0;
        animator.SetBool("isWalking", isWalking);
        
        // Orienter le personnage dans la direction du mouvement
        if (movement.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720f * Time.deltaTime);
        }
    }
    
    // Cette fonction est appelée automatiquement par le Root Motion
    void OnAnimatorMove()
    {
        // Appliquer le mouvement de l'animation multiplié par notre vitesse
        Vector3 velocity = animator.deltaPosition * moveSpeed / Time.deltaTime;
        velocity.y = 0; // On ne veut pas monter
        
        transform.position += velocity * Time.deltaTime;
    }
}