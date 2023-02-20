using System.Collections;
using UnityEngine;

public class Card_AttackUp : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");
        
        World_StatusEffectLibrary.AddStatusEffect(Player.GetEntity(), StatusEffect_ID.ATTACK);

        yield return null;
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}