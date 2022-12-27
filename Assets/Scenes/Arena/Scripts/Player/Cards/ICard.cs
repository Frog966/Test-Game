using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//! This is the base interface for card scripts
//! Do not modify this script unless you're sure you want to affect ALL cards
public interface ICard {
    //! Stuff that needs to be defined for cards to work
    Player_Cards Resolver { get; }
    Player_Energy EnergyHandler { get; }
    Card_Behaviour Behaviour { get; }

    // Card stats
    string ID { get; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    string Desc { get; }
    int Dmg { get; }
    int Cost { get; }

    Text ID_T { get; }
    Text Desc_T { get; } 
    Text Dmg_T { get; } 
    Text Cost_T { get; }

    // Misc.
    GameObject GameObj { get; }

    // Constructor
    void Setup(Player player);

    // Card effect
    // Always pass effect in resolver.PlayCard() instead of calling Effect()
    IEnumerator Effect();
}