using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_PunchRush : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(World_Grid.GetEntityGridPos(Player.GetEntity()), new List<Vector2Int> { new Vector2Int(1, 0) }, false);

        for (int i = 0; i < cardStats.NoOfHits_Final; i++) {
            World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

            yield return World_Grid.Combat.FlashHere(posList, 0.1f);
            yield return AnimHandler.WaitForSeconds(0.1f); // Add a slight delay so animation looks nice
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}