using UnityEngine;
using Game.Unit;

// Player class and IEnemy interface will inherit this
public interface IEntity {
    Faction Faction { get; }

    int Health { get; set; }
    int HealthMax { get; set; }

    GameObject GameObj { get; } // Use this to get the entity's gameObject

    void OnHit();
    void OnDeath();
}