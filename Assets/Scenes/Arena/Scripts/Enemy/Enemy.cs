using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEntity interface to Enemy script so that we can use a single type to handle turns
public class Enemy : MonoBehaviour, IEntity {
    [SerializeField] private string id; // Used by World_EnemyLibrary
    [SerializeField] private string enemyName;
    [SerializeField] private IEnemyAI ai; // The AI this enemy will be using

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

    // Getters
    public string GetID() { return id; }
    public string GetEnemyName() { return enemyName; }

    // A bit jank but the simplest way to dump only World_Grid into enemy AI
    public void Setup(World_Grid _gridHandler) {
        ai.Setup(_gridHandler);
    }

    public List<Task> ReturnCurrentTurn() { return ai.ReturnCurrentTurn(); }

    void Awake() {
        // Sanity checks
        if (ai == null) ai = this.GetComponent<IEnemyAI>();

        _healthMax = _health = 500; // Setting up enemy's health properties
    }
}