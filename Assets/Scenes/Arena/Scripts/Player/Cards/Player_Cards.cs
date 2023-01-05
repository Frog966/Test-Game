using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Contains the player's card-related functionality such as deck, hand, graveyard and exile as well as card effects
//! Card parents are always enabled to allow for animations. The cards themselves will be disabled
public class Player_Cards : MonoBehaviour {
    // References
    [SerializeField] private Player_Energy energyHandler;
    [SerializeField] private Player_CardLibrary cardLibrary;

    [Header("Card Parents")]
    [SerializeField] private Transform cardParent_Play; // A parent to temporarily hold the played card as well as act as their tween targets
    [SerializeField] private Transform cardParent_Deck, cardParent_Hand, cardParent_GY, cardParent_Exile;

    [Header("Text Fields")]
    [SerializeField] private Text textDeck;
    [SerializeField] private Text textHand, textGY, textExile;

    private List<ICard> gy = new List<ICard>(); // Lists all card prefabs in GY
    private List<ICard> deck = new List<ICard>(); // Lists all card prefabs in deck
    private List<ICard> hand = new List<ICard>(); // Lists all card prefabs in hand
    private List<ICard> exile = new List<ICard>(); // Lists all card prefabs in exile

    // Getters
    public Transform GetPlayParent() { return cardParent_Play; }
    public List<ICard> GetGY() { return gy; }
    public List<ICard> GetDeck() { return deck; }
    public List<ICard> GetExile() { return exile; }

    public void Shuffle() {
        System.Random rng = new System.Random();
        int n = deck.Count;

        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            ICard value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value;
        }
    }

    public IEnumerator Draw(int num = 1) {
        float tweenDur = 0.2f;
        float tweenDelay = 0.1f;

        // Debug.Log("Player draws " + num + " card" + (num != 1 ? "s" : "") + "!");

        for (int i = 0; i < num; i++) {
            // If deck has no cards, shuffle GY into deck
            if (deck.Count < 1) {
                // If GY has cards, shuffle GY into deck
                // else (deck and GY have no cards at this point), break loop since you can't draw any more cards
                //! Only shuffle GY into deck when the deck is empty which means you have to draw cards 1 by 1 until deck is empty first
                if (gy.Count > 0) {
                    // Add each card listed in gy to deck then clear gy 
                    foreach (ICard card in gy) {
                        deck.Add(card);
                        card.GameObj.transform.SetParent(cardParent_Deck);
                        card.GameObj.transform.localPosition = Vector3.zero; // Reset card pos when moved to deck
                    }

                    gy.Clear();
                    UpdateNoOfCards_GY(); // Update GY count

                    Shuffle(); // Shuffle the deck

                    yield return World_AnimHandler.WaitForSeconds(0.2f); //! TODO: Play shuffle anim here

                    UpdateNoOfCards_Deck(); // Update deck count

                    yield return World_AnimHandler.WaitForSeconds(0.2f);
                }
                else { break; }
            }

            // We're updating the deck and hand lists as we move cards between them each time we draw a card
            // Therefore, only the top card of the deck (deck[0]) is accessed
            ICard currCard = deck[0]; 
            GameObject currCardObj = currCard.GameObj;

            hand.Add(currCard);
            deck.RemoveAt(0);

            currCardObj.transform.SetParent(cardParent_Hand);
            currCardObj.SetActive(true);

            // Debug.Log("Draw " + (i + 1));

            // Card position in hand
            Vector2 newPos = GetCardHandPos(currCardObj.transform, cardParent_Hand.childCount - 1);

            currCardObj.transform.DOLocalMove(newPos, tweenDur);
            currCard.EventHandler.SetCardLocalStartPos(newPos);

            UpdateNoOfCards_Hand();
            UpdateNoOfCards_Deck();

            yield return World_AnimHandler.WaitForSeconds(tweenDelay);
        }

        yield return World_AnimHandler.WaitForSeconds(tweenDur - tweenDelay);
    }

    // Discard cards
    public IEnumerator Discard(List<ICard> discardedCards) {
        if (discardedCards.Count > 0) {
            float tweenDur = 0.2f;

            gy = new List<ICard>(gy.Concat(discardedCards).ToList()); // Pass discarded cards to GY
            hand = new List<ICard>(hand.Where((card) => !discardedCards.Contains(card)).ToList()); // Remove discarded cards from hand

            UpdateNoOfCards_Hand();

            // Move cards to cardParent_GY
            //--------------------------------------------------------------------------------------------------------------------------------------------------
            foreach (ICard card in discardedCards) {
                Transform cardTrans = card.GameObj.transform;

                cardTrans.SetParent(cardParent_GY);
                cardTrans.DOLocalMove(Vector2.zero, tweenDur);
            }
            
            yield return World_AnimHandler.WaitForSeconds(tweenDur);

            foreach (ICard card in discardedCards) { card.GameObj.SetActive(false); }

            UpdateNoOfCards_GY();
            //--------------------------------------------------------------------------------------------------------------------------------------------------
        }
        else { yield return null; } // Don't do anything if there is nothing to discard
    }

    public IEnumerator DiscardHand() { yield return Discard(hand); }

    // Adds a card prefab to cardParent_Deck + Registers card to deck card list
    //! These cards are temporary and will be cleared out when the battle ends unless called by Deck.InstantiateDeck()
    public void CreateCard_Deck(string id) {
        ICard cardPrefab = cardLibrary.GetCardByID(id);

        if (cardPrefab != null) {
            GameObject newCard = GameObject.Instantiate(cardPrefab.GameObj, cardParent_Deck);
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
            deck.Add(newCard.GetComponent<ICard>());
            
            UpdateNoOfCards_Deck();
        }
        else { Debug.LogError("Cannot add card ID '" + id + "' to deck!"); }
    }

    // Adds a card prefab to cardParent_GY + Registers card to gy card list
    //! These cards are temporary and will be cleared out when the battle ends
    public void CreateCard_GY(string id) {
        ICard cardPrefab = cardLibrary.GetCardByID(id);

        if (cardPrefab != null) {
            GameObject newCard = GameObject.Instantiate(cardPrefab.GameObj, cardParent_GY);
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
            gy.Add(newCard.GetComponent<ICard>());
            
            UpdateNoOfCards_GY();
        }
        else { Debug.LogError("Cannot add card ID '" + id + "' to GY!"); }
    }

    // Clears out all parents and recreate the deck
    // Called at World_Turns.StartEncounter()
    public void ResetCards() {
        gy.Clear();
        deck.Clear();
        hand.Clear();
        exile.Clear();

        foreach (Transform child in cardParent_Deck) { child.SetParent(cardParent_GY); } // Move cards from cardParent_Deck to cardParent_GY first

        deck = new List<ICard>(Deck.GetDeckCards()); // Reregisters all cards from deckCards into deck

        // Move cards that are registered as default deck cards back into cardParent_Deck
        foreach (ICard card in deck) { 
            Transform currCard = card.GameObj.transform;

            currCard.SetParent(cardParent_Deck); 
            currCard.localPosition = Vector3.zero;
        }
        
        // Destroy every other card
        foreach (Transform child in cardParent_GY) { Destroy(child.gameObject); }
        foreach (Transform child in cardParent_Play) { Destroy(child.gameObject); }
        foreach (Transform child in cardParent_Hand) { Destroy(child.gameObject); }
        foreach (Transform child in cardParent_Exile) { Destroy(child.gameObject); }

        UpdateNoOfCards_GY();
        UpdateNoOfCards_Hand();
        UpdateNoOfCards_Deck();
        UpdateNoOfCards_Exile();
    }

    // Just a wrapper so that Player_Cards is the one performing the coroutine and not the card itself
    public void ResolveCard(ICard playedCard) { StartCoroutine(ResolveCard_Anim(playedCard)); }

    private IEnumerator ResolveCard_Anim(ICard playedCard) {
        World_AnimHandler.isAnimating = true;
        
        Transform cardTrans = playedCard.GameObj.transform;

        // Move card to cardParent_Play
        //--------------------------------------------------------------------------------------------------------------------------------------------------
        float tweenDur1 = 0.2f;

        cardTrans.SetParent(cardParent_Play); // Store the played card here temporarily
        cardTrans.DOLocalMove(Vector2.zero, tweenDur1);

        energyHandler.DecreaseEnergy(playedCard.Cost); // Reduce player's energy based on played card's cost
        hand.RemoveAt(hand.FindIndex((card) => playedCard == card)); // Remove from hand list as card is tweening to cardParent_Play
        UpdateNoOfCards_Hand();

        yield return World_AnimHandler.WaitForSeconds(tweenDur1);

        ArrangeCardsInHand(); // Arrange cards in hand after moving played card out of hand
        //--------------------------------------------------------------------------------------------------------------------------------------------------

        // Play card's effect
        yield return playedCard.Effect();

        if (playedCard.IsExiled) {
            // Move card to cardParent_Exile
            //--------------------------------------------------------------------------------------------------------------------------------------------------
            float tweenDur3 = 0.2f;

            cardTrans.SetParent(cardParent_Exile);
            cardTrans.DOLocalMove(Vector2.zero, tweenDur3);

            yield return World_AnimHandler.WaitForSeconds(tweenDur3);

            exile.Add(playedCard); // Add to exile list

            UpdateNoOfCards_Exile();
            //--------------------------------------------------------------------------------------------------------------------------------------------------
        }
        else {
            // Move card to cardParent_GY
            //--------------------------------------------------------------------------------------------------------------------------------------------------
            float tweenDur2 = 0.2f;

            cardTrans.SetParent(cardParent_GY);
            cardTrans.DOLocalMove(Vector2.zero, tweenDur2);
            
            yield return World_AnimHandler.WaitForSeconds(tweenDur2);

            gy.Add(playedCard); // Add to GY list

            UpdateNoOfCards_GY();
            //--------------------------------------------------------------------------------------------------------------------------------------------------
        }

        // Finish resolving card
        //--------------------------------------------------------------------------------------------------------------------------------------------------        
        cardTrans.gameObject.SetActive(false); // Disable card after card effect
        // cardTrans.localPosition = Vector3.zero; // Set card pos
        
        // UpdateNoOfCards_Hand();
        // UpdateNoOfCards_Deck(); // Also update deck card counter in case played card adds cards to deck

        World_AnimHandler.isAnimating = false;
        //--------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void ArrangeCardsInHand() {
        for (int i = 0; i < cardParent_Hand.childCount; i ++) {
            Transform card = cardParent_Hand.GetChild(i);
            Vector2 newPos = GetCardHandPos(card, i);

            card.GetComponent<ICard>().EventHandler.SetCardLocalStartPos(newPos); // Set card position in hand
            card.transform.DOLocalMove(newPos, 0.2f);
        }
    }

    private Vector2 GetCardHandPos(Transform card, int multiplier) {
        ICard cardScript = card.GetComponent<ICard>();

        return new Vector2((-(cardScript.UIHandler.GetWidthOffset() * multiplier) - cardScript.UIHandler.GetWidthOffset()), 0.0f);
    }
    
    private void UpdateNoOfCards_GY() { textGY.text = gy.Count.ToString(); }
    private void UpdateNoOfCards_Deck() { textDeck.text = deck.Count.ToString(); }
    private void UpdateNoOfCards_Hand() { textHand.text = hand.Count.ToString(); }
    private void UpdateNoOfCards_Exile() { textExile.text = exile.Count.ToString(); }

    public static class Deck {
        private static Player_Cards cardsHandler;
        private static Player_CardLibrary cardLibrary;

        private static List<string> deckIDs = new List<string>(); // Holds all of player deck cards as IDs
        private static List<ICard> deckCards = new List<ICard>(); // A record of every single card belonging to the deck. Any card not under this list will be destroyed when resetting the cards

        // Getters
        public static List<string> GetDeckIDs() { return deckIDs; }
        public static List<ICard> GetDeckCards() { return deckCards; }

        public static void SetDeck(List<string> newDeck) {
            // Remove any invalid card IDs from newDeck
            System.Predicate<string> doesLibraryNotHaveID = (id) => { return !cardLibrary.DoesLibraryHaveID(id); };
            newDeck.RemoveAll(doesLibraryNotHaveID);

            deckIDs = newDeck;
        }

        // This function instantiates card GOs into cardParent_Deck as well as records them into deckCards
        public static void InstantiateDeck() {
            cardsHandler.deck.Clear();

            // Instantiate card objs into deck
            foreach (string id in deckIDs) { cardsHandler.CreateCard_Deck(id); }

            deckCards = new List<ICard>(cardsHandler.deck); // Registers all cards in deckCards
        }

        // The only way to permanently add a card to deck
        public static void AddCardToDeck(ICard card) {
            deckIDs.Add(card.ID);
            cardsHandler.CreateCard_Deck(card.ID);
        }

        // The only way to permanently remove a card from deck
        // No need to remove the card from any lists as ResetCards() will handle that if called on start of encounter
        public static void RemoveCardFromDeck(ICard card) {
            deckIDs.Remove(card.ID);
        }

        // Just to define cardsHandler and cardLibrary
        public static void Setup(Player_Cards _handler, Player_CardLibrary _library) { 
            cardLibrary = _library; 
            cardsHandler = _handler; 
        }
    }

    void Awake() {
        //! Sanity Checks
        if (energyHandler == null) energyHandler = this.gameObject.GetComponent<Player_Energy>();
        if (cardLibrary == null) cardLibrary = this.gameObject.GetComponent<Player_CardLibrary>();

        // Card parents must be set to true
        cardParent_GY.gameObject.SetActive(true);
        cardParent_Hand.gameObject.SetActive(true);
        cardParent_Deck.gameObject.SetActive(true);
        cardParent_Exile.gameObject.SetActive(true);

        Deck.Setup(this, cardLibrary);
    }

    // Start is called before the first frame update
    void Start() {
        // Testing only!
        Deck.SetDeck(new List<string>() {
            "Cannon",
            "Cannon",
            "Cannon",
            "Cannon",
            "Minigun",
            "Minigun",
            "Minigun",
            "Minigun",
            "LookItUp",
            "LookItUp",
        });

        ResetCards();
        Deck.InstantiateDeck(); // Instantiate the deck
    }
}