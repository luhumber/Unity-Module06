using UnityEngine;

public class Key : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null) {
                inventory.AddKey();
                Destroy(gameObject);
            }
        }
    }
}