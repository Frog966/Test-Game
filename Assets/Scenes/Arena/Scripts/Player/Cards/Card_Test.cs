using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Test : MonoBehaviour, ICard {
    private static Player_Cards _resolver;
    private static Player_Energy _energyHandler;

    [SerializeField] private Card_Behaviour _behaviour;
    private Vector2 _startLocalPos;
    
    [Header("Card Stats")]
    [SerializeField] private bool _isTemp;
    [SerializeField] private string _id, _desc;
    [SerializeField] private int _dmg, _cost;
    
    [Header("Card Text Fields")]
    [SerializeField] private Text id_T;
    [SerializeField] private Text desc_T, dmg_T, cost_T;

    //! Stuff that needs to be defined for cards to work
    public Player_Cards resolver { get => _resolver; }
    public Player_Energy energyHandler { get => _energyHandler; }
    public Card_Behaviour behaviour { get => _behaviour; }

    // Card settings
    public bool isTemp { get => _isTemp; set => _isTemp = value; } // In case temporary cards get generated during combat

    // Card stats
    public string id { get => _id; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    public string desc { get => _desc; }
    public int dmg { get => _dmg; }
    public int cost { get => _cost; }

    // Misc.
    public GameObject gameObj { get => this.gameObject; }
    
    // Constructor
    public void Setup(Player player) {
        if (!_resolver) _resolver = player.CardsHandler();
        if (!_energyHandler) _energyHandler = player.EnergyHandler();

        // Set up Card_Behaviour
        if (!_behaviour) _behaviour = this.GetComponent<Card_Behaviour>();
        if (!_behaviour) _behaviour = this.gameObject.AddComponent<Card_Behaviour>() as Card_Behaviour;
        
        if (!this.GetComponent<BoxCollider2D>()) Debug.LogError(this.gameObject.name + "does not have a box collider!");

        id_T.text = _id;
        desc_T.text = _desc;
        dmg_T.text = _dmg.ToString();
        cost_T.text = _cost.ToString();
    }
    
    // Do not call Effect(). Card_Behaviour will call it instead
    // Does not require World_AnimHandler.instance.isAnimating as Card_Behaviour will handle that
    public IEnumerator Effect() {
        Debug.Log("Card is being played!");
        
        yield return _resolver.Draw(1);
    }
}