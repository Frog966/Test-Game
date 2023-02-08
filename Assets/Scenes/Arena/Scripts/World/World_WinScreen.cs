using UnityEngine;
using UnityEngine.SceneManagement;

public class World_WinScreen : MonoBehaviour {
    [SerializeField] private GameObject winScreen;

    public void EnableScreen() { winScreen.SetActive(true); }
    public void DisableScreen() { winScreen.SetActive(false); }

    public void RestartScene() { 
        if (!AnimHandler.isAnimating) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    void Awake() {
        DisableScreen();
    }
}