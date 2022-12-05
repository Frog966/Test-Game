// using System.Collections;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

//! Contains every single card in the game in library variable
public class Player_CardLibrary : MonoBehaviour {
    private Dictionary<string, Card> library;

    public Card GetCardByName(string cardName) { return library[cardName]; }
    public void PlayCardByName(string cardName) { GetCardByName(cardName).Play(); }
    public Dictionary<string, Card>.KeyCollection GetAllCardIDs() { return library.Keys; }
    public bool DoesLibraryHaveID(string id) { return library.ContainsKey(id); }

    void Awake() {
        InitLibrary();
    }

    private void InitLibrary() {
        // Just to streamline setting card name
        KeyValuePair<string, Card> CreateEntry(string cardName, Action cardEffect) {
            return new KeyValuePair<string, Card> (cardName, new Card(cardName, cardEffect));
        }
        
        // Initialize the library dictionary here
        // All card functionality is here
        library = new Dictionary<string, Card>(
            new [] {
                CreateEntry(
                    "Test Card",
                    () => {
                        Debug.Log("Card Played!");
                    }
                )
            }
        );
    }
}

// A container for all card functionality in the dictionary
// Certain variables must be public for Player_Card
public class Card {
    // Card stats
    public string id; // Doubles as card name
    public int cost;
    public string desc;
    public int dmg;

    public bool isTemp; // In case cards get generated during combat

    private Task effect; // Used to save card functionality assigned by library

    public Card(string newName, Action newEffect) {
        id = newName;
        effect = new Task(newEffect);
    }

    public void Play() {
        effect.Start();
        effect.Wait();
    }
}