using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_Cleaver : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;
    [SerializeField] private AudioClip audio_Slash;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(
            World_Grid.GetEntityGridPos(cardStats.Player.GetEntity()),
            new List<Vector2Int>() { 
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(2, 0),
                new Vector2Int(2, 1),
                new Vector2Int(2, -1),
            },
            false
        );

        World_Grid.Combat.HitHere(Faction.ALLY, posList, cardStats.Dmg);
        AudioHandler.PlayClip(audio_Slash);

        yield return World_Grid.Combat.FlashHere(posList);
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}