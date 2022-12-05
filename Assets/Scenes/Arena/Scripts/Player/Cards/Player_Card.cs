using System;
using System.Collections.Generic;
using UnityEngine;

// Stores card functionality from Player_CardLibrary's Card class
public class Player_Card : MonoBehaviour {
    private Card cardStats; // A reference to the card
    
    public void Init(Card card) {
        cardStats = card;
    }

    public void Play() {
        cardStats.Play();
    }
}