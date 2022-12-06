using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Contains the player's card-related functionality such as deck, hand, graveyard and exile
public class Player_Cards : MonoBehaviour {
    // References
    private Player player;
    private Player_CardLibrary cardLibrary;

    // Holds all of player cards as IDs
    private List<string> deckBase = new List<string>() {
        "Test Card",
        "Test Card"
    }; 

    public GameObject cardPrefab;
    public Transform cardParent_Deck, cardParent_Hand, cardParent_GY;
    public Text textDeck, textHand, textGY;

    //! Adding these lists together should be the every card belonging to the player
    public List<Player_Card> deck = new List<Player_Card>(); // Lists all card prefabs in deck
    public List<Player_Card> hand = new List<Player_Card>(); // Lists all card prefabs in hand
    public List<Player_Card> gy = new List<Player_Card>(); // Lists all card prefabs in GY

    private void MoveCardTo() {
        
    }

    public void Shuffle() {
        System.Random rng = new System.Random();
        int n = deck.Count;

        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Player_Card value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value;
        }
    }

    public void Draw(int num = 1) {
        // Debug.Log("Player draws " + num + " card" + (num != 1 ? "s" : "") + "!");

        for (int i = 0; i < num; i++) {
            // If deck has no cards, shuffle GY into deck
            if (deck.Count < 1) {
                // If GY has cards, shuffle GY into deck
                // else (deck and GY have no cards at this point), break loop since you can't draw any more cards
                //! Only shuffle GY into deck when the deck is empty which means you have to draw cards 1 by 1 until deck is empty first
                if (gy.Count > 0) {
                    // Add each card listed in gy to deck then clear gy 
                    foreach (Player_Card card in gy) {
                        deck.Add(card);
                        card.transform.SetParent(cardParent_Deck);
                    }

                    gy.Clear();
                    
                    Shuffle(); // Shuffle the deck
                    UpdateNoOfCards_GY(); // Update GY text
                }
                else {
                    break;
                }
            }

            // We're updating the deck and hand lists as we move cards between them each time we draw a card
            // Therefore, only the top card of the deck (deck[0]) is accessed
            Player_Card currCard = deck[0]; 

            hand.Add(currCard);
            currCard.transform.SetParent(cardParent_Hand);
            currCard.transform.localPosition = new Vector2(0 - ((currCard.GetComponent<RectTransform>().rect.width / 2) * (cardParent_Hand.childCount - 1)), 0); // Set card position

            deck.RemoveAt(0);

            // Debug.Log("Draw " + (i + 1));
        }

        UpdateNoOfCards_Deck();
        UpdateNoOfCards_Hand();
    }

    void Awake() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();
        if (cardLibrary == null) cardLibrary = this.gameObject.GetComponent<Player_CardLibrary>();
        
        // Clear out any children in cardParent_Deck
        foreach (Transform child in cardParent_Deck) {
            child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
            Destroy(child.gameObject);
        }
        
        // Clear out any children in cardParent_Hand
        foreach (Transform child in cardParent_Hand) {
            child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
            Destroy(child.gameObject);
        }
        
        // Clear out any children in cardParent_GY
        foreach (Transform child in cardParent_GY) {
            child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
            Destroy(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        // Disable these gameobjects to prevent cards from rendering
        cardParent_GY.gameObject.SetActive(false);
        cardParent_Deck.gameObject.SetActive(false);

        // Remove any invalid card IDs from deckBase
        System.Predicate<string> doesLibraryNotHaveID = (id) => { return !cardLibrary.DoesLibraryHaveID(id); };
        deckBase.RemoveAll(doesLibraryNotHaveID);

        // Instantiate card objs into deck
        foreach (string id in deckBase) { CreateCardObj_Deck(id); }

        UpdateNoOfCards_GY();
        UpdateNoOfCards_Deck();
    }

    // Adds a card prefab to cardParent_Deck + Registers card to deck card list
    private void CreateCardObj_Deck(string id) {
        GameObject newCardObj = GameObject.Instantiate(cardPrefab, cardParent_Deck);
        Player_Card newCard = newCardObj.GetComponent<Player_Card>();

        newCard.Init(cardLibrary.GetCardByName(id)); // Attach correct card functionality to card obj
        deck.Add(newCard);
    }

    // Adds a card prefab to cardParent_GY + Registers card to gy card list
    private void CreateCardObj_GY(string id) {
        GameObject newCardObj = GameObject.Instantiate(cardPrefab, cardParent_GY);
        Player_Card newCard = newCardObj.GetComponent<Player_Card>();

        newCard.Init(cardLibrary.GetCardByName(id)); // Attach correct card functionality to card obj
        gy.Add(newCard);
    }
    
    private void UpdateNoOfCards_Deck() {
        textDeck.text = deck.Count.ToString();
    }
    
    private void UpdateNoOfCards_Hand() {
        textHand.text = hand.Count.ToString();
    }
    
    private void UpdateNoOfCards_GY() {
        textGY.text = gy.Count.ToString();
    }
}