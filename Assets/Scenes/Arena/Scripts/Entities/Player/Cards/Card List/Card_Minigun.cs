using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Minigun : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;
    [SerializeField] private AudioClip audio_Shoot;

    List<Vector2Int> posList;

    public void DisplayRange() { 
        posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);

        World_Grid.Combat.FlashHere_Start(posList); 
    }

    public void StopDisplayRange() { World_Grid.Combat.FlashHere_Stop(posList); }

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        Player.GetEntity().PlayAnimation("Shoot Start");
        
        yield return AnimHandler.WaitForCurrentAnim(Player.GetEntity().GetAnimator());

        for (int i = 0; i < cardStats.NoOfHits_Final; i++) {
            posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);
            World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

            Player.GetEntity().PlayAnimation("Shoot End");
            AudioHandler.PlayClip(audio_Shoot);
            
            yield return World_Grid.Combat.FlashHere(posList, 0.1f);
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}