using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEntity interface to Enemy script so that we can use a single type to handle turns
public class Enemy : MonoBehaviour, IEntity {
    [SerializeField] private string id; // Used by World_EnemyLibrary
    [SerializeField] private string enemyName;
    
    private World_Grid gridHandler;
    private World_Turns turnsHandler;
    private IEnemyAI ai;

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
    public World_Grid GridHandler() { return gridHandler; }
    public World_Turns TurnsHandler() { return turnsHandler; }

    public void Setup(World_Grid _gridHandler, World_Turns _turnsHandler) {
        gridHandler = _gridHandler;
        turnsHandler = _turnsHandler;

        ai = this.GetComponent<IEnemyAI>();
    }

    public List<Task> ReturnCurrentTurn() { return ai.ReturnCurrentTurn(); }

    void Awake() {
        _healthMax = _health = 500; // Setting up enemy's health properties
    }
}