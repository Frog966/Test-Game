using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//! Contains every single card in the game in library variable
public class Player_CardLibrary : MonoBehaviour {
    [SerializeField] private Player player;

    private Dictionary<string, ICard> library = new Dictionary<string, ICard>();
    
    public ICard GetCardByID(string id) {
        if (library.ContainsKey(id)) { return library[id]; }
        else {
            Debug.LogWarning("Card ID '" + id + "' not found in library!");

            return null;
        }
    }
    
    public bool DoesLibraryHaveID(string id) { return library.ContainsKey(id); }

    public List<ICard> GetLibrary_Common() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.COMMON).ToList(); }
    public List<ICard> GetLibrary_Uncommon() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.UNCOMMON).ToList(); }
    public List<ICard> GetLibrary_Rare() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.RARE).ToList(); }
    public List<ICard> GetLibrary_Legendary() { return library.Values.ToList().Where((card) => card.Rarity == CardRarity.LEGENDARY).ToList(); }

    private void InitLibrary() {
        foreach (ICard card in Resources.LoadAll("Cards", typeof(ICard)).Cast<ICard>().ToArray()) {
            if (!library.ContainsKey(card.ID)) {
                card.Setup(player); // Setup card as we're adding it to the library
                library.Add(card.ID, card);
            }
            else { Debug.LogWarning("There is a duplicate card ID '" + card.ID + "' found in the Resources folder!"); }
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!player) player = this.gameObject.GetComponent<Player>();
        
        InitLibrary();
    }
}