using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Card;

//! Contains every single card in the game in library variable
public class Player_CardLibrary : MonoBehaviour {
    [SerializeField] private Player player;

    private Dictionary<string, Card_Stats> library = new Dictionary<string, Card_Stats>();
    
    public Card_Stats GetCardByID(string id) {
        if (library.ContainsKey(id)) { return library[id]; }
        else {
            Debug.LogWarning("Card ID '" + id + "' not found in library!");

            return null;
        }
    }
    
    public bool DoesLibraryHaveID(string id) { return library.ContainsKey(id); }

    public List<Card_Stats> GetLibrary_Common() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.COMMON).ToList(); }
    public List<Card_Stats> GetLibrary_Uncommon() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.UNCOMMON).ToList(); }
    public List<Card_Stats> GetLibrary_Rare() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.RARE).ToList(); }
    public List<Card_Stats> GetLibrary_Legendary() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.LEGENDARY).ToList(); }

    private void InitLibrary() {
        Card_Stats.Setup(player); // Setup player ref for all cards

        foreach (Card_Stats card in Resources.LoadAll("Cards", typeof(Card_Stats)).Cast<Card_Stats>().ToArray()) {
            if (!library.ContainsKey(card.ID)) { library.Add(card.ID, card); }
            else { Debug.LogWarning("There is a duplicate card ID '" + card.ID + "' found in the Resources folder!"); }
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!player) player = this.gameObject.GetComponent<Player>();
        
        InitLibrary();
    }
}