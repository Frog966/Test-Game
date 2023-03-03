using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Test : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

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

        posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(Player.GetEntity()), false);
        List<Entity> hitEntities = World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

        yield return World_Grid.Combat.FlashHere(posList);

        if (hitEntities.Count > 0) { 
            World_StatusEffectLibrary.AddStatusEffect(Player.GetEntity(), StatusEffect_ID.ATTACK);
        }

        // On-hit effects here
        foreach (Entity entity in hitEntities) {
            World_StatusEffectLibrary.AddStatusEffect(entity, StatusEffect_ID.DEFENCE);
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}