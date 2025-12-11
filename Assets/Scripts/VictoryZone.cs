using UnityEngine;

public class VictoryZone : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player wins!");
            GameManager.TriggerVictory();
        }
    }
}