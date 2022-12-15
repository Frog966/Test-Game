using UnityEngine;
using Game.Unit;

// Player class and IEnemy interface will inherit this
public interface IEntity {
    Faction Faction { get; } // The entity's faction
    GameObject GameObj { get; } // The entity's gameObject

    int Health { get; set; }
    int HealthMax { get; set; }

    void OnHit();
    void OnDeath();
}