using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// The base script for entities
// Contains anything an entity has that would be interacted with by other scripts
public class Entity : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Text health_T;
    [SerializeField] private Faction faction;
    [SerializeField] private Transform statusEffectParent;
    
    [Header("Entity Stats")]
    public int healthMax; // Allow get and set
    [SerializeField] private int health; // Only allow get
    public List<IStatusEffect> statusEffect_List = new List<IStatusEffect>();

    private Stack<Action> todo_OnDeath = new Stack<Action>();

    // Getters
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetHealth() { return health; }
    public Faction GetFaction() { return faction; }
    public Transform GetSEParent() { return statusEffectParent; }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    
    public int GetStackableSECounter(StatusEffect_ID id) { 
        IStatusEffect_Stackable se = (IStatusEffect_Stackable)statusEffect_List.Find((el) => el.ID == id);

        return se != null ? se.Counter : 0; 
    }
    
    // Calculates the final dmg that will be dealt by this entity by including SEs
    public int GetFinalDamage(int baseDmg) { 
        int modifier_Att = GetStackableSECounter(StatusEffect_ID.ATTACK) * 10;
        int finalDmg = baseDmg + modifier_Att;

        // Debug.Log("baseDmg: " + baseDmg);
        // Debug.Log("modifier_Att: " + modifier_Att);
        // Debug.Log("GetFinalDamage: " + finalDmg);

        return finalDmg > 0 ? finalDmg : 0;
    }

    public IEnumerator StartTurn() {
        // Create a duplicate statusEffect_List to iterate through it
        foreach (IStatusEffect se in statusEffect_List.ToList()) {
            se.StartOfTurn();

            yield return World_AnimHandler.WaitForSeconds(0.1f);
        }
    }

    public IEnumerator EndTurn() {
        // Create a duplicate statusEffect_List to iterate through it
        foreach (IStatusEffect se in statusEffect_List.ToList()) {
            se.EndOfTurn();

            yield return World_AnimHandler.WaitForSeconds(0.1f);
        }
    }

    public virtual void OnHit(int baseDmg) {
        int modifier_Def = GetStackableSECounter(StatusEffect_ID.DEFENCE) * 10;
        int finalDmg = baseDmg - modifier_Def;
        
        // Debug.Log("OnHit: " + baseDmg + ", " + finalDmg);

        health -= finalDmg > 0 ? finalDmg : 0;

        UpdateHealthUI();
    }

    // Adds a new action that the entity will perform on death
    public void AddToDo_OnDeath(Action newAct) { todo_OnDeath.Push(newAct); }

    public void OnDeath() {
        foreach (Action act in todo_OnDeath) act(); // Perform every action added to todo_OnDeath

        this.gameObject.SetActive(false);
    }

    // Update health text
    private void UpdateHealthUI() { health_T.text = health.ToString(); }

    void Awake() {
        while (statusEffectParent.childCount > 0) { Destroy(statusEffectParent.GetChild(0).gameObject); }
        
        health = healthMax;
        UpdateHealthUI();
    }
}