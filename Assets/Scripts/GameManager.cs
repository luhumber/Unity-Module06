using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField] private Animator victoryAnimator;
    
    private static GameManager instance;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public static void TriggerGameOver() {
        if (instance != null) {
            instance.gameOverAnimator.SetTrigger("PlayFade");
        }
    }
    
    public static void TriggerVictory() {
        if (instance != null) {
            instance.victoryAnimator.SetTrigger("PlayFade");
        }
    }
}