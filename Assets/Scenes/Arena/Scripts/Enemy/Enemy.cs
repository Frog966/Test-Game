using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an Entity interface to Enemy script so that we can use a single type to handle turns
public class Enemy : MonoBehaviour, Entity {
    public string id; // Used by World_EnemyLibrary
    public string enemyName;

    // Properties
    private int _health, _healthMax;
    public int Health {
        get => _health;
        set => _health = value;
    }
    public int HealthMax  {
        get => _healthMax;
        set => _healthMax = value;
    }

    void Awake() {
        _healthMax = _health = 500; // Setting up enemy's health properties
    }
}