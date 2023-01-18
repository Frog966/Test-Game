using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Test : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require World_AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        // Debug.Log(this + " is being played!");
        
        yield return cardStats.CardHandler.Draw(1);
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}