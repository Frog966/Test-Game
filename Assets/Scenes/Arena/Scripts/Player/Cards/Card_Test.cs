using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Test : MonoBehaviour, ICard {
    private static Player_Cards resolver;
    private static Player_Energy energyHandler;
    [SerializeField] private Card_Behaviour behaviour;

    private Vector2 startLocalPos;
    
    [Header("Card Stats")]
    [SerializeField] private string id, desc;
    [SerializeField] private int dmg, cost;
    
    [Header("Card Text Fields")]
    [SerializeField] private Text id_T;
    [SerializeField] private Text desc_T, dmg_T, cost_T;

    //! Stuff that needs to be defined for cards to work
    public Player_Cards Resolver { get => resolver; }
    public Player_Energy EnergyHandler { get => energyHandler; }
    public Card_Behaviour Behaviour { get => behaviour; }

    // Card stats
    public string ID { get => id; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    public string Desc { get => desc; }
    public int Dmg { get => dmg; }
    public int Cost { get => cost; }

    // Card text fields
    public Text ID_T { get => id_T; }
    public Text Desc_T { get => desc_T; }
    public Text Dmg_T { get => dmg_T; }
    public Text Cost_T { get => cost_T; }

    // Misc.
    public GameObject GameObj { get => this.gameObject; }
    
    // Constructor
    public void Setup(Player player) {
        if (!resolver) resolver = player.CardsHandler();
        if (!energyHandler) energyHandler = player.EnergyHandler();

        // Set up Card_Behaviour
        if (!behaviour) behaviour = this.GetComponent<Card_Behaviour>();
        if (!behaviour) behaviour = this.gameObject.AddComponent<Card_Behaviour>() as Card_Behaviour;
        
        if (!this.GetComponent<BoxCollider2D>()) Debug.LogError(this.gameObject.name + "does not have a box collider!");

        id_T.text = id;
        desc_T.text = desc;
        dmg_T.text = dmg.ToString();
        cost_T.text = cost.ToString();
    }
    
    // Do not call Effect(). Card_Behaviour will call it instead
    // Does not require World_AnimHandler.isAnimating as Card_Behaviour will handle that
    public IEnumerator Effect() {
        Debug.Log("Card is being played!");
        
        yield return resolver.Draw(1);
    }
}