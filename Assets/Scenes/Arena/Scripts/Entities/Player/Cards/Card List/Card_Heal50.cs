using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_Heal50 : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        Entity player = cardStats.Player.GetEntity();
        player.SetHealth(player.GetHealth() + 50);

        yield return null;
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}