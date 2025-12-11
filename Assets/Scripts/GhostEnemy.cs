using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GhostEnemy : MonoBehaviour {
    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float catchDistance = 1.5f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask obstacleMask;
    
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    
    [Header("Chase")]
    [SerializeField] private float chaseTime = 5f;
    
    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing = false;
    private bool isReturning = false;
    private float chaseTimer;
    private int currentPointIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;
    private Vector3 startPosition;
    private bool hasCaughtPlayer = false;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        startPosition = transform.position;
        
        if (patrolPoints.Length > 0) {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    void Update() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeeTarget(player.position);
        
        if (isReturning) {
            animator.SetBool("isWalking", true);
            
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
                isReturning = false;
                currentPointIndex = 0;
                if (patrolPoints.Length > 0) {
                    agent.SetDestination(patrolPoints[currentPointIndex].position);
                }
            }
            return;
        }
        
        if (isChasing) {
            chaseTimer -= Time.deltaTime;
            
            if (distanceToPlayer <= catchDistance) {
                CatchPlayer();
                return;
            }
            
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
            
            if (chaseTimer <= 0) {
                StopChasing();
            }
        } else if (canSeePlayer && distanceToPlayer <= detectionRange) {
            StartChasing();
        } else {
            Patrol();
        }
    }

    void StartChasing() {
        isChasing = true;
        chaseTimer = chaseTime;
        isWaiting = false;
    }

    void StopChasing() {
        isChasing = false;
        isReturning = true;
        
        agent.SetDestination(startPosition);
    }

    void CatchPlayer() {
        if (hasCaughtPlayer) return;
        hasCaughtPlayer = true;
        
        Debug.Log("Player caught!");
        
        GameManager.TriggerGameOver();
        
        Invoke("RestartScene", 3f);
    }

    void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AlertToPlayer() {
        if (!isReturning) {
            isChasing = true;
            chaseTimer = chaseTime * 5;
            isWaiting = false;
        }
    }

    void Patrol() {
        if (patrolPoints.Length == 0) return;
        
        if (isWaiting) {
            waitTimer -= Time.deltaTime;
            animator.SetBool("isWalking", false);
            
            if (waitTimer <= 0) {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPointIndex].position);
            }
        } else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
        } else {
            animator.SetBool("isWalking", true);
        }
    }

    bool CanSeeTarget(Vector3 targetPosition) {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToTarget > detectionRange)
            return false;
        
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > fieldOfView / 2f)
            return false;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToTarget, out hit, distanceToTarget, obstacleMask)) {
            return false;
        }
        
        return true;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
        
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * forward;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        if (patrolPoints != null && patrolPoints.Length > 1) {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++) {
                if (patrolPoints[i] != null) {
                    int nextIndex = (i + 1) % patrolPoints.Length;
                    if (patrolPoints[nextIndex] != null) {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                    }
                }
            }
        }
    }
}