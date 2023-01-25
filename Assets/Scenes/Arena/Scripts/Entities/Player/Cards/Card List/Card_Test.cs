using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

public class Card_Test : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require World_AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(cardStats.Player.GetEntity()), false);
        List<Entity> hitEntities = World_Grid.Combat.HitHere(Faction.ALLY, posList, cardStats.Dmg);

        yield return World_Grid.Combat.FlashHere(posList);

        if (hitEntities.Count > 0) { World_StatusEffectLibrary.AddStatusEffect(cardStats.Player.GetEntity(), StatusEffect_ID.ATTACK_UP); }

        // On-hit effects here
        foreach (Entity entity in hitEntities) {
            World_StatusEffectLibrary.AddStatusEffect(entity, StatusEffect_ID.ATTACK_UP);
        }
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}