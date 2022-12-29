using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Unit;

public class Card_Cannon : MonoBehaviour, ICard {
    private static Player player;
    private static Player_Cards cardHandler;
    private static Player_Energy energyHandler;
    
    [Header("Card Scripts")]
    [SerializeField] private Card_UI uiHandler;
    [SerializeField] private Card_Events eventHandler;

    [Header("Card Stats")]
    [SerializeField] private bool isExiled;
    [SerializeField] private Sprite image;
    [SerializeField] private string id, desc;
    [SerializeField] private int cost, dmg, noOfHits = 1;

    // Properties
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player_Cards CardHandler { get => cardHandler; }
    public Player_Energy EnergyHandler { get => energyHandler; }
    public Card_UI UIHandler { get => uiHandler; }
    public Card_Events EventHandler { get => eventHandler; }

    public bool IsExiled { get => isExiled; }
    public string ID { get => id; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    public string Desc { get => desc; }
    public int Dmg { get => dmg; }
    public int Cost { get => cost; }
    public int NoOfHits { get => noOfHits; }
    public Sprite Image { get => image; }

    public GameObject GameObj { get => this.gameObject; }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    // Constructor
    public void Setup(Player _player) {
        if (!cardHandler) cardHandler = _player.CardsHandler();
        if (!energyHandler) energyHandler = _player.EnergyHandler();
        if (!uiHandler) uiHandler = this.transform.GetChild(0).GetComponent<Card_UI>();
        if (!eventHandler) eventHandler = this.transform.GetChild(0).GetComponent<Card_Events>();

        player = _player;
        uiHandler.Setup(this);
    }
    
    // Do not call Effect(). Card_Events will call it instead
    // Does not require World_AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Right(World_Grid.GetEntityGridPos(player.GetEntity()), false);

        World_Grid.Combat.HitHere(Faction.ALLY, posList, dmg);

        yield return World_Grid.Combat.FlashHere(posList);
    }
}