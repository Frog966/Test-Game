using UnityEngine;
using UnityEngine.SceneManagement;

public class World_LoseScreen : MonoBehaviour {
    [SerializeField] private GameObject loseScreen;

    public void EnableScreen() { loseScreen.SetActive(true); }
    public void DisableScreen() { loseScreen.SetActive(false); }

    public void RestartScene() { 
        if (!AnimHandler.isAnimating) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    void Awake() {
        DisableScreen();
    }
}