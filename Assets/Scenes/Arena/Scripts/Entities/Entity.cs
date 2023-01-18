using System;
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
    public List<StatusEffect_Info> statusEffect_List = new List<StatusEffect_Info>();

    private Stack<Action> todo_OnDeath = new Stack<Action>();

    // Getters
    public int GetHealth() { return health; }
    public Faction GetFaction() { return faction; }

    public virtual void OnHit(int baseDmg) {
        health -= baseDmg;

        health_T.text = health.ToString(); // Update health text
    }

    // Adds a new action that the entity will perform on death
    public void AddToDo_OnDeath(Action newAct) { todo_OnDeath.Push(newAct); }

    public void OnDeath() {
        foreach (Action act in todo_OnDeath) act(); // Perform every action added to todo_OnDeath

        this.gameObject.SetActive(false);
    }

    void Awake() {
        while (statusEffectParent.childCount > 0) { Destroy(statusEffectParent.GetChild(0).gameObject); }
    }
}