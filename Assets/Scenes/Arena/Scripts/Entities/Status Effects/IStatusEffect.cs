using UnityEngine;

public interface IStatusEffect {
    StatusEffect_ID ID { get; }
    StatusEffect_UI UI { get; } // The UI this SE is attached to
    Sprite Sprite { get; }
    Entity Entity { get; } // The entity this SE is attached to
    GameObject GameObject { get; } // The GO this SE is attached to

    void RemoveThisSE() { Entity.statusEffect_List.Remove(this); }

    // Remove self from entity's SE list
    void StartOfTurn() { RemoveThisSE(); }
}

public interface IStatusEffect_Stackable : IStatusEffect {
    int Counter { get; set; }

    void AddCounter(int i = 1) {
        Counter += i;
        UI.UpdateCounter(Counter);
    }
    
    void MinusCounter(int i = 1) {
        Counter -= i;

        if (Counter <= 0) Counter = 0;
        
        UI.UpdateCounter(Counter);
    }

    // Reduce stack. If <= 0, remove self from entity's SE list
    // This is an Explicit Interface Implementation overriding the StartOfTurn() from IStatusEffect
    void IStatusEffect.StartOfTurn() {
        MinusCounter();

        if (Counter <= 0) { RemoveThisSE(); }
    }
}

public interface IStatusEffect_Timer : IStatusEffect {
    int TimerMax { get; }
    int Timer { get; set; }

    void ResetTimer() {
        Timer = TimerMax;
        
        UI.UpdateCounter(Timer);
    }
    
    void MinusTimer(int i = 1) {
        Timer -= i;
        
        if (Timer <= 0) Timer = 0;
        
        UI.UpdateCounter(Timer);
    }

    void Effect();

    // Reduce timer. If <= 0, perform an action then remove self from entity's SE list
    // This is an Explicit Interface Implementation overriding the StartOfTurn() from IStatusEffect
    void IStatusEffect.StartOfTurn() {
        MinusTimer(Timer - 1);
        
        if (Timer <= 0) {
            Effect();
            RemoveThisSE();
        }
    }
}