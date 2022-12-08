using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEntity interface to Player script so that we can use a single type to handle turns
public class Player : MonoBehaviour, IEntity {
    //! Handler scripts
    private Player_Cards _cardsHandler;
    private Player_Energy _energyHandler;
    private Player_Movement _movementHandler;
    private Player_KeyPresses _keyPressHandler;

    // The player object
    public GameObject playerObj;

    public Vector2Int playerCoor; // Keeps track of player coordinates on grid

    // Player stats
    public int energyMaxTrue = 3, energy, energyMax;  // Energy amounts
    public int moveCostTrue = 1, moveCost; // Movement cost

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

    // Handler Getters
    public Player_Cards CardsHandler() { return _cardsHandler; }
    public Player_Energy EnergyHandler() { return _energyHandler; }
    public Player_Movement MovementHandler() { return _movementHandler; }
    public Player_KeyPresses KeyPressHandler() { return _keyPressHandler; }

    public void StartTurn() {
        // Debug.Log("Player starting turn!");

        CardsHandler().Draw(5);
    }

    void Awake() {
        // Sanity checks
        if (_cardsHandler == null) _cardsHandler = this.gameObject.GetComponent<Player_Cards>();
        if (_energyHandler == null) _energyHandler = this.gameObject.GetComponent<Player_Energy>();
        if (_movementHandler == null) _movementHandler = this.gameObject.GetComponent<Player_Movement>();
        if (_keyPressHandler == null) _keyPressHandler = this.gameObject.GetComponent<Player_KeyPresses>();

        // Setting up player's health properties
        _healthMax = _health = 500;
    }
}