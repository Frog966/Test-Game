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

    [SerializeField] private Transform cardParent_Play; // A parent to temporarily hold the played card as well as act as their tween targets
    [SerializeField] private Transform cardParent_Deck, cardParent_Hand, cardParent_GY, cardParent_Exile;
    [SerializeField] private Text textDeck, textHand, textGY, textExile;

    private List<ICard> gy = new List<ICard>(); // Lists all card prefabs in GY
    private List<ICard> deck = new List<ICard>(); // Lists all card prefabs in deck
    private List<ICard> hand = new List<ICard>(); // Lists all card prefabs in hand
    private List<ICard> exile = new List<ICard>(); // Lists all card prefabs in exile

    // Getters
    public Transform GetPlayParent() { return cardParent_Play; }

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
                    
                    Shuffle(); // Shuffle the deck
                    UpdateNoOfCards_GY(); // Update GY count
                    UpdateNoOfCards_Deck(); // Update deck count
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
            Vector2 newPos = new Vector2(0.0f - ((currCardObj.GetComponent<RectTransform>().rect.width / 2.0f) * (cardParent_Hand.childCount - 1)), 0.0f);

            currCardObj.transform.DOLocalMove(newPos, tweenDur);
            currCardObj.GetComponent<Card_Behaviour>().SetStartLocalPos(newPos);

            yield return World_AnimHandler.WaitForSeconds(tweenDelay);
        }

        yield return World_AnimHandler.WaitForSeconds(tweenDur - tweenDelay);

        UpdateNoOfCards_Hand();
    }

    public IEnumerator PlayCard(ICard playedCard, bool isExiled = false) {
        playedCard.GameObj.transform.SetParent(cardParent_Play); // Store the played card here temporarily

        ArrangeCardsInHand(); // Arrange cards in hand after moving played card out of hand

        energyHandler.DecreaseEnergy(playedCard.Cost); // Reduce player's energy based on played card's cost

        yield return playedCard.Effect(); // Play card effect

        if (isExiled) {
            yield return MoveCardToGY(playedCard); // Move card to GY
        }
        else {
            yield return ExileCard(playedCard); // Move card to exile
        }
    }

    // Adds a card prefab to cardParent_Deck + Registers card to deck card list
    //! These cards are temporary and will be cleared out when the battle ends unless called by Deck.InstantiateDeck()
    public void CreateCard_Deck(string id) {
        ICard cardPrefab = cardLibrary.GetCardByID(id);

        if (cardPrefab != null) {
            GameObject newCard = GameObject.Instantiate(cardPrefab.GameObj, cardParent_Deck);
            deck.Add(newCard.GetComponent<ICard>());
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
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
            gy.Add(newCard.GetComponent<ICard>());
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
            UpdateNoOfCards_GY();
        }
        else { Debug.LogError("Cannot add card ID '" + id + "' to GY!"); }
    }

    // Clears out all parents and recreate the deck
    // Called at World_Turns.SetupEncounter()
    public void ResetCards() {
        void CheckCard(Transform currCard) {
            ICard card = currCard.GetComponent<ICard>();

            if (Deck.GetDeckCards().Contains(card)) { currCard.SetParent(cardParent_Deck); }
            else { Destroy(currCard.gameObject); }
        }

        gy.Clear();
        deck.Clear();
        hand.Clear();
        exile.Clear();

        deck = new List<ICard>(Deck.GetDeckCards()); // Registers all cards in deckCards

        foreach (Transform child in cardParent_GY) { CheckCard(child); }
        foreach (Transform child in cardParent_Deck) { CheckCard(child); }
        foreach (Transform child in cardParent_Hand) { CheckCard(child); }
        foreach (Transform child in cardParent_Exile) { CheckCard(child); }
        
        UpdateNoOfCards_GY();
        UpdateNoOfCards_Hand();
        UpdateNoOfCards_Deck();
        UpdateNoOfCards_Exile();
    }

    private IEnumerator MoveCardToGY(ICard playedCard) {
        Transform cardTrans = playedCard.GameObj.transform;

        float tweenDur = 0.2f;
        Vector3 endPoint = cardTrans.InverseTransformPoint(cardParent_GY.position) + cardTrans.localPosition + new Vector3(playedCard.Behaviour.GetWidthOffset(), 0.0f, 0.0f);

        // Debug.Log("1: " + cardParent_GY.localPosition);
        // Debug.Log("2: " + cardTrans.localPosition);
        // Debug.Log("3: " + new Vector3(playedCard.behaviour.GetWidthOffset(), 0.0f, 0.0f));

        cardTrans.DOLocalMoveX(endPoint.x, tweenDur).SetEase(Ease.OutQuint);
        cardTrans.DOLocalMoveY(endPoint.y, tweenDur);
        
        yield return World_AnimHandler.WaitForSeconds(tweenDur);

        cardTrans.SetParent(cardParent_GY);
        gy.Add(playedCard); // Add to GY list

        UpdateNoOfCards_GY();
        FinishPlayingCard(playedCard);
    }

    private IEnumerator ExileCard(ICard playedCard) {        
        Transform cardTrans = playedCard.GameObj.transform;

        yield return World_AnimHandler.WaitForSeconds(0.2f);

        cardTrans.SetParent(cardParent_Exile);
        exile.Add(playedCard); // Add to exile list

        UpdateNoOfCards_Exile();
        FinishPlayingCard(playedCard);
    }

    private void FinishPlayingCard(ICard playedCard) {
        Transform cardTrans = playedCard.GameObj.transform;

        hand.RemoveAt(hand.FindIndex((card) => playedCard == card)); // Remove from hand list
        
        cardTrans.gameObject.SetActive(false); // Disable card after card effect
        cardTrans.localPosition = Vector3.zero; // Set card pos
        
        UpdateNoOfCards_Hand();
        UpdateNoOfCards_Deck(); // Also update deck card counter in case played card adds cards to deck

        World_AnimHandler.isAnimating = false;
    }

    private void ArrangeCardsInHand() {
        for (int i = 0; i < cardParent_Hand.childCount; i ++) {
            Transform card = cardParent_Hand.GetChild(i);
            Vector2 newPos = new Vector2(0.0f - ((card.GetComponent<RectTransform>().rect.width / 2.0f) * i), 0.0f); // Set card position in hand

            card.GetComponent<Card_Behaviour>().SetStartLocalPos(newPos);
            card.transform.DOLocalMove(newPos, 0.2f);
        }
    }
    
    private void UpdateNoOfCards_GY() { textGY.text = gy.Count.ToString(); }
    private void UpdateNoOfCards_Deck() { textDeck.text = deck.Count.ToString(); }
    private void UpdateNoOfCards_Hand() { textHand.text = hand.Count.ToString(); }
    private void UpdateNoOfCards_Exile() { textHand.text = hand.Count.ToString(); }

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
            "Test",
            "Test",
            "Test",
            "Test",
            "Test",
            "Test",
        });

        ResetCards();
        Deck.InstantiateDeck(); // Instantiate the deck
    }
}