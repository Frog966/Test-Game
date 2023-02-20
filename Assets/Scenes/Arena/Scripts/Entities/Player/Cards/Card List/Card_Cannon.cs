using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_Cannon : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);

        Player.GetEntity().PlayAnimation("Shoot Start");
        
        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());

        World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

        yield return World_Grid.Combat.FlashHere(posList);
        
        Player.GetEntity().PlayAnimation("Shoot End");

        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}