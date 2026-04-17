using UnityEngine;

public class World_QuitScreen : MonoBehaviour {
    [SerializeField] private GameObject quitScreen;

    public void EnableScreen() { quitScreen.SetActive(true); }
    public void DisableScreen() { quitScreen.SetActive(false); }

    public void QuitGame() { 
        if (!AnimHandler.isAnimating) { Application.Quit(); }
    }

    void Awake() {
        DisableScreen();
    }
}