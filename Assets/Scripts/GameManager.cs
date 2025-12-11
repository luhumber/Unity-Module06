using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField] private Animator victoryAnimator;
    
    [Header("Audio")]
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource victorySound;
    
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
            instance.gameOverSound.Play();
            instance.gameOverAnimator.SetTrigger("PlayFade");
        }
    }
    
    public static void TriggerVictory() {
        if (instance != null) {
            instance.victorySound.Play();
            instance.victoryAnimator.SetTrigger("PlayFade");
        }
    }
}