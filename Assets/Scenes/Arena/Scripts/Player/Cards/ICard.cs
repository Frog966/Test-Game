using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//! This is the base interface for card scripts
//! Do not modify this script unless you're sure you want to affect ALL cards
public interface ICard {
    //! Stuff that needs to be defined for cards to work
    Player_Cards resolver { get; }
    Player_Energy energyHandler { get; }
    Card_Behaviour behaviour { get; }

    // Card settings
    bool isTemp { get; set; } // In case temporary cards get generated during combat

    // Card stats
    string id { get; } // Doubles as card name. Please ensure there are no duplicates or Player_CardLibrary will not instantiate properly
    string desc { get; }
    int dmg { get; }
    int cost { get; }

    Text id_T { get; }
    Text desc_T { get; } 
    Text dmg_T { get; } 
    Text cost_T { get; }

    // Misc.
    GameObject gameObj { get; }

    // Constructor
    void Setup(Player player);

    // Card effect
    // Always pass effect in resolver.PlayCard() instead of calling Effect()
    IEnumerator Effect();
}