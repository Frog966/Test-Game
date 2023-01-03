using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Test : MonoBehaviour, ICard {
    private static Player_Cards cardHandler;
    private static Player_Energy energyHandler;
    
    [Header("Card Scripts")]
    [SerializeField] private Card_UI uiHandler;
    [SerializeField] private Card_Events eventHandler;

    [Header("Card Stats")]
    [SerializeField] private CardRarity rarity;
    [SerializeField] private bool isExiled;
    [SerializeField] private bool isUpgraded;
    [SerializeField] private Sprite image;
    [SerializeField] private string id, desc;
    [SerializeField] private int cost, dmg, noOfHits = 1;

    // Properties
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player_Cards CardHandler { get => cardHandler; }
    public Player_Energy EnergyHandler { get => energyHandler; }
    public Card_UI UIHandler { get => uiHandler; }
    public Card_Events EventHandler { get => eventHandler; }

    public CardRarity Rarity { get => rarity; }
    public bool IsExiled { get => isExiled; }
    public bool IsUpgraded { get => isUpgraded; }
    public string ID { get => id; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    public string Desc { get => desc; }
    public int Dmg { get => dmg; }
    public int Cost { get => cost; }
    public int NoOfHits { get => noOfHits; }
    public Sprite Image { get => image; }

    public GameObject GameObj { get => this.gameObject; }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    // Constructor
    public void Setup(Player player) {
        if (!cardHandler) cardHandler = player.CardsHandler();
        if (!energyHandler) energyHandler = player.EnergyHandler();
        if (!uiHandler) uiHandler = this.transform.GetChild(0).GetComponent<Card_UI>();
        if (!eventHandler) eventHandler = this.transform.GetChild(0).GetComponent<Card_Events>();

        uiHandler.Setup(this);
    }
    
    // Do not call Effect(). Card_Events will call it instead
    // Does not require World_AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        // Debug.Log(this + " is being played!");
        
        yield return cardHandler.Draw(1);
    }
}