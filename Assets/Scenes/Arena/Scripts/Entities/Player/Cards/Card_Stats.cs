using System.Collections;
using UnityEngine;
using Game.Card;

public class Card_Stats : MonoBehaviour {
    private static Player player;
    private static Player_Cards cardHandler;
    private static Player_Energy energyHandler;
    
    [Header("Card Scripts")]
    [SerializeField] private Card_UI uiHandler;
    [SerializeField] private Card_Events eventHandler;
    [SerializeField] private ICardEffect effect;

    [Header("Card Stats")]
    public bool isPlayable = true; // Determine if this card is played during combat
    [SerializeField] private CardRarity rarity;
    [SerializeField] private bool isExiled;
    [SerializeField] private bool isUpgraded;
    [SerializeField] private Sprite image;
    [SerializeField] private string id, desc;
    [SerializeField] private int cost, dmg, noOfHits = 1;

    // Properties
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player Player { get => player; }
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
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    // Constructor
    public void Setup(Player _player) {
        if (!player) player = _player;
        if (!cardHandler) cardHandler = player.CardsHandler();
        if (!energyHandler) energyHandler = player.EnergyHandler();

        uiHandler.Setup(this);
    }

    // Copy the card passed except CardType as param then setup
    public void Copy(Card_Stats cardStats) {        
        rarity = cardStats.Rarity;
        isExiled = cardStats.IsExiled;
        isUpgraded = cardStats.IsUpgraded;
        id = cardStats.ID;
        desc = cardStats.Desc;
        dmg = cardStats.Dmg;
        cost = cardStats.Cost;
        noOfHits = cardStats.NoOfHits;
        image = cardStats.Image;

        Setup(player);
    }

    public IEnumerator Effect() {
        if (effect != null) { yield return effect.Effect(); }
        else yield return null;
    }

    private void Awake() {
        if (!uiHandler) uiHandler = this.GetComponent<Card_UI>();
        if (!eventHandler) eventHandler = this.GetComponent<Card_Events>();
        if (effect == null) effect = this.GetComponent<ICardEffect>();
    }
}