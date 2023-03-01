using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_TutorialScreen : MonoBehaviour {
    [SerializeField] private GameObject screen;

    public void DisableScreen() { screen.SetActive(false); }

    void Awake() {
        screen.SetActive(true);
    }
}
