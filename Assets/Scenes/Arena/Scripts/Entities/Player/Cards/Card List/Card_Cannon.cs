using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Cannon : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;
    [SerializeField] private AudioClip audio_Shoot;

    private List<Vector2Int> posList;

    public void DisplayRange() { 
        posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);

        World_Grid.Combat.FlashHere_Start(posList); 
    }

    public void StopDisplayRange() { World_Grid.Combat.FlashHere_Stop(posList); }

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);
        
        Player.GetEntity().PlayAnimation("Shoot Start");
        
        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());

        World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

        yield return World_Grid.Combat.FlashHere(posList);
        
        Player.GetEntity().PlayAnimation("Shoot End");
        AudioHandler.PlayClip(audio_Shoot);

        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());
        // yield return AnimHandler.WaitForSeconds(5.0f);
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}