using UnityEngine;

public class GargoyleEnemy : MonoBehaviour {
    private bool hasAlerted = false;
    private float alertCooldown = 0f;
    [SerializeField] private float cooldownTime = 3f;

    void Update() {
        if (alertCooldown > 0) {
            alertCooldown -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && alertCooldown <= 0) {
            AlertGhosts();
            alertCooldown = cooldownTime;
        }
    }

    void AlertGhosts() {
        GhostEnemy[] ghosts = FindObjectsOfType<GhostEnemy>();
        
        foreach (GhostEnemy ghost in ghosts) {
            ghost.AlertToPlayer();
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Transform detectionZone = transform.Find("DetectionZone");
        if (detectionZone != null) {
            BoxCollider col = detectionZone.GetComponent<BoxCollider>();
            if (col != null) {
                Gizmos.matrix = detectionZone.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);
            }
        }
    }
}