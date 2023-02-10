using System.Collections;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an Entity interface to Player script so that we can use a single type to handle turns
public class Player : MonoBehaviour {
    [SerializeField] private Entity entity; // The player object
    [SerializeField] private UnityEngine.UI.Text health_T;

    private static Player Inst;

    [Header("Non-player Handler Scripts")]
    [SerializeField] private World_Turns turnsHandler;

    [Header("Player Handler Scripts")]
    [SerializeField] private Player_Cards _cardsHandler;
    [SerializeField] private Player_Energy _energyHandler;
    [SerializeField] private Player_Movement _movementHandler;
    [SerializeField] private Player_KeyPresses _keyPressHandler;

    [Header("UI Stuff")]
    private int bits = 0; // The player's currency
    [SerializeField] private UnityEngine.UI.Text bitsText;

    // Handler Getters
    public Player_Cards CardsHandler() { return _cardsHandler; }
    public Player_Energy EnergyHandler() { return _energyHandler; }
    public Player_Movement MovementHandler() { return _movementHandler; }
    public Player_KeyPresses KeyPressHandler() { return _keyPressHandler; }

    // Getters
    public int GetBits() { return bits; }
    public static Entity GetEntity() { return Inst.entity; }

    // Setters
    public void SetBits(int newBits) { 
        bits = newBits;
        UpdateBitsText();
    }

    // Player's start turn function
    // Contains anything that triggers at start of turn
    public IEnumerator StartTurn() {
        Debug.Log("Player starts turn!");

        _energyHandler.ResetEnergy();
        _movementHandler.ResetMoveCost();

        yield return CardsHandler().Draw(5);

        AnimHandler.isAnimating = false; //! The game will not stop animating until the player's turn
    }

    // Player's end turn function
    // Used at the "End Turn" button
    public void EndTurn() { StartCoroutine(EndTurn_Anim()); }

    private IEnumerator EndTurn_Anim() {
        if (!AnimHandler.isAnimating) {
            AnimHandler.isAnimating = true;
            
            Debug.Log("Player ends turn!");

            AudioHandler.PlayClip_Button();

            yield return _cardsHandler.DiscardHand(); // Discard player's hand

            StartCoroutine(turnsHandler.EndTurn());
        }
    }

    private void UpdateBitsText() { bitsText.text = bits.ToString(); }

    void Awake() {
        // Instance declaration
        if (Inst != null && Inst != this) { Destroy(this); }
        else { Inst = this; }

        // Non-player handlers sanity checks
        if (turnsHandler == null) { turnsHandler = this.transform.parent.GetComponentInChildren(typeof(World_Turns)) as World_Turns; }

        // Player handlers sanity checks
        if (_cardsHandler == null) _cardsHandler = this.gameObject.GetComponent<Player_Cards>();
        if (_energyHandler == null) _energyHandler = this.gameObject.GetComponent<Player_Energy>();
        if (_movementHandler == null) _movementHandler = this.gameObject.GetComponent<Player_Movement>();
        if (_keyPressHandler == null) _keyPressHandler = this.gameObject.GetComponent<Player_KeyPresses>();

        UpdateBitsText();
    }
}