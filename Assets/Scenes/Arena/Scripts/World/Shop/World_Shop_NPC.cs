using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_Shop_NPC : MonoBehaviour {
    [SerializeField] private GameObject textBox;
    [SerializeField] private TMPro.TMP_Text textBox_Text;

    private float timerDur = 5.0f;
    private float timerCount = 0.0f;
    private bool isTextBoxOpen = false;

    private IEnumerator test;

    public void PopUp_NotEnoughMoney() {
        textBox_Text.text = "You poor. Lol";

        EnableTextBox();
    }

    private void EnableTextBox() {
        timerCount = 0.0f;
        isTextBoxOpen = true;

        textBox.SetActive(true);
    }

    private void DisableTextBox() {
        isTextBoxOpen = false;
        
        textBox.SetActive(false);
    }

    void Awake() {
        DisableTextBox();
    }

    void Update() {
        if (isTextBoxOpen) {
            timerCount += Time.deltaTime;

            if (timerCount >= timerDur) { DisableTextBox(); }
        }
    }
}
