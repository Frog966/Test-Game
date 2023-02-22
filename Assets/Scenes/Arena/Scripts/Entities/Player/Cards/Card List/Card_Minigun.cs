using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Minigun : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        Player.GetEntity().PlayAnimation("Shoot Start");
        
        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());

        for (int i = 0; i < cardStats.NoOfHits_Final; i++) {
            List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);
            World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

            Player.GetEntity().PlayAnimation("Shoot End");
            
            yield return World_Grid.Combat.FlashHere(posList, 0.1f);
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}