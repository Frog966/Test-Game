using UnityEngine;
using Game.Unit;

// C# does not allow multiple inheritance so we're attaching an IEntity interface to Player script so that we can use a single type to handle turns
public class Player : MonoBehaviour, IEntity {
    [Header("Player Handler Scripts")]
    [SerializeField] private Player_Cards _cardsHandler;
    [SerializeField] private Player_Energy _energyHandler;
    [SerializeField] private Player_Movement _movementHandler;
    [SerializeField] private Player_KeyPresses _keyPressHandler;

    [Header("Non-player Handler Scripts")]
    [SerializeField] private World_Turns turnsHandler;

    // The player object
    [SerializeField] private GameObject playerObj;

    [Header("Player Stats")]
    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private int healthMax;
    [SerializeField] private int health;

    public int Health {
        get => health;
        set => health = value;
    }
    
    public int HealthMax  {
        get => healthMax;
        set => healthMax = value;
    }
    
    public Faction Faction { get => Faction.ALLY; }
    public GameObject GameObj { get => playerObj; }

    public void OnHit(int damage) {
        Debug.Log("Player is hit!");

        health -= damage;
    }

    public void OnDeath() {
        Debug.Log("Player is dead!");
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    // Player stats
    public int energyMaxTrue = 3, energy, energyMax;  // Energy amounts
    public int moveCostTrue = 1, moveCost; // Movement cost

    // Handler Getters
    public Player_Cards CardsHandler() { return _cardsHandler; }
    public Player_Energy EnergyHandler() { return _energyHandler; }
    public Player_Movement MovementHandler() { return _movementHandler; }
    public Player_KeyPresses KeyPressHandler() { return _keyPressHandler; }


    // Player's start turn function
    // Contains anything that triggers at start of turn
    public void StartTurn() {
        Debug.Log("Player starts turn!");

        // Debug.Log("Player starting turn!");

        _movementHandler.ResetMoveCost();
        StartCoroutine(CardsHandler().Draw(5));
    }

    // Player's end turn function
    // Contains anything that triggers at end of turn
    // Also used at the "End Turn" button
    public void EndTurn() {
        if (!World_AnimHandler.instance.isAnimating) {
            Debug.Log("Player ends turn!");

            turnsHandler.EndTurn();
        }
    }

    void Awake() {
        // Player handlers sanity checks
        if (_cardsHandler == null) _cardsHandler = this.gameObject.GetComponent<Player_Cards>();
        if (_energyHandler == null) _energyHandler = this.gameObject.GetComponent<Player_Energy>();
        if (_movementHandler == null) _movementHandler = this.gameObject.GetComponent<Player_Movement>();
        if (_keyPressHandler == null) _keyPressHandler = this.gameObject.GetComponent<Player_KeyPresses>();
        
        // Non-player handlers sanity checks
        if (turnsHandler == null) { turnsHandler = this.transform.parent.GetComponentInChildren(typeof(World_Turns)) as World_Turns; }

        // Setting up player's health properties
        healthMax = health = 500;
    }
}