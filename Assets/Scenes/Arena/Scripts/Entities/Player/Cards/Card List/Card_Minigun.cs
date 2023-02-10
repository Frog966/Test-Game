using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_Minigun : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);

        for (int i = 0; i < cardStats.NoOfHits; i++) {
            World_Grid.Combat.HitHere(Faction.ALLY, posList, cardStats.Dmg);

            yield return World_Grid.Combat.FlashHere(posList, 0.1f);
            yield return AnimHandler.WaitForSeconds(0.1f); // Add a slight delay so animation looks nice
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}