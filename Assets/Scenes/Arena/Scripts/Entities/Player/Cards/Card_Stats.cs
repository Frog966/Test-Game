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
    [SerializeField] private CanvasGroup canvasGroup; // Used to change the entire card's alpha
    [SerializeField] private ICardEffect effect;

    [Header("Card Stats")]
    public bool isPlayable = true; // Determine if this card is played during combat
    [SerializeField] private CardRarity rarity;
    [SerializeField] private bool isExiled;
    [SerializeField] private bool isUpgraded;
    [SerializeField] private Sprite image;
    [SerializeField] private string id, desc;
    [SerializeField] private int cost, dmg, noOfHits = 1; // The base stats
    private int _cost, _dmg, _noOfHits; // The derived stats after math

    // Properties
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    // public Player Player { get => player; }
    public Player_Cards CardHandler { get => cardHandler; }
    public Player_Energy EnergyHandler { get => energyHandler; }
    public Card_UI UIHandler { get => uiHandler; }
    public Card_Events EventHandler { get => eventHandler; }
    public CanvasGroup CanvasGroup { get => canvasGroup; }

    public CardRarity Rarity { get => rarity; }
    public bool IsExiled { get => isExiled; }
    public bool IsUpgraded { get => isUpgraded; }
    public string ID { get => id; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    public string Desc { get => desc; }
    public int Dmg { get => _dmg; }
    public int Cost { get => _cost; }
    public int NoOfHits { get => _noOfHits; }
    public Sprite Image { get => image; }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    // Sets up static stuff for Card_Stats
    public static void Setup(Player _player) {
        if (!player) player = _player;
        if (!cardHandler) cardHandler = player.CardsHandler();
        if (!energyHandler) energyHandler = player.EnergyHandler();
    }

    // Preferably called after Awake()
    public void UpdateUI() {
        _dmg = Player.GetEntity().GetFinalDamage(dmg);
        _cost = cost;
        _noOfHits = noOfHits;

        uiHandler.UpdateText();
    }

    // Copy the card passed except CardType as param then setup
    // Used when generating card rewards
    public void Copy(Card_Stats cardStats) {        
        rarity = cardStats.rarity;
        isExiled = cardStats.isExiled;
        isUpgraded = cardStats.isUpgraded;
        id = cardStats.id;
        desc = cardStats.desc;
        image = cardStats.image;
        dmg = cardStats.dmg;
        cost = cardStats.cost;
        noOfHits = cardStats.noOfHits;
    }

    public IEnumerator Effect() {
        if (effect != null) { yield return effect.Effect(); }
        else yield return null;
    }

    void Awake() {
        if (!uiHandler) uiHandler = this.GetComponent<Card_UI>();
        if (effect == null) effect = this.GetComponent<ICardEffect>();
        if (!eventHandler) eventHandler = this.GetComponent<Card_Events>();
    }

    void Start() {
        UpdateUI();
    }
}