using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum CardRarity {
    COMMON,
    UNCOMMON,
    RARE,
    LEGENDARY,
}

//! This is the base interface for card scripts
//! Do not modify this script unless you're sure you want to affect ALL cards
public interface ICard {
    // Handlers
    Player_Cards CardHandler { get; }
    Player_Energy EnergyHandler { get; }

    // Card Scripts
    Card_UI UIHandler { get; }
    Card_Events EventHandler { get; }

    // Card Stats
    CardRarity Rarity { get; }
    bool IsExiled { get; }
    bool IsUpgraded { get; }
    string ID { get; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    string Desc { get; }
    int Dmg { get; }
    int Cost { get; }
    int NoOfHits { get; }
    Sprite Image { get; }

    // Constructor
    void Setup(Player player);

    GameObject GameObj { get; }

    // Card effect
    // Always pass effect in CardHandler.ResolveCard() instead of calling Effect()
    IEnumerator Effect();
}