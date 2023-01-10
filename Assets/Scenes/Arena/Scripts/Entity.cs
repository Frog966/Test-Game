using UnityEngine;
using Game.Unit;

// The base script for entities
// Contains anything an entity has that would be interacted with by other scripts
public class Entity : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Text health_T;
    [SerializeField] private Faction faction;
    
    [Header("Entity Stats")]
    public int healthMax; // Allow get and set
    [SerializeField] private int health; // Only allow get

    // Getters
    public int GetHealth() { return health; }
    public Faction GetFaction() { return faction; }

    public virtual void OnHit(int baseDmg) {
        health -= baseDmg;

        health_T.text = health.ToString(); // Update health text
    }

    public virtual void OnDeath() {
        this.gameObject.SetActive(false);
    }
}