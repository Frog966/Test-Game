using UnityEngine;

// Player class and IEnemy interface will inherit this
public interface IEntity {
    int Health { get; set; }
    int HealthMax { get; set; }

    GameObject GameObj { get; } // Use this to get the entity's gameObject
}