using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Contains the player's card-related functionality such as deck, hand, graveyard and exile as well as card effects
//! Card parents are always enabled to allow for animations. The cards themselves will be disabled
public class Player_Cards : MonoBehaviour {
    // References
    private Player player;
    [SerializeField] private Player_CardLibrary cardLibrary;

    // Holds all of player deck cards as IDs
    private List<string> deckBase = new List<string>();

    [SerializeField] private Transform cardParent_Play; // A parent to temporarily hold the played card as well as act as their tween targets
    [SerializeField] private Transform cardParent_Deck, cardParent_Hand, cardParent_GY;
    [SerializeField] private Text textDeck, textHand, textGY;

    //! Adding these lists together should be the every card belonging to the player
    [SerializeField] private List<ICard> deck = new List<ICard>(); // Lists all card prefabs in deck
    [SerializeField] private List<ICard> hand = new List<ICard>(); // Lists all card prefabs in hand
    [SerializeField] private List<ICard> gy = new List<ICard>(); // Lists all card prefabs in GY

    private Queue<IEnumerator> cardQueue = new Queue<IEnumerator>();

    // Getters
    public Transform GetPlayParent() { return cardParent_Play; }

    // Remove any invalid card IDs from deckBase
    private void ValidateDeck() {
        System.Predicate<string> doesLibraryNotHaveID = (id) => { return !cardLibrary.DoesLibraryHaveID(id); };
        deckBase.RemoveAll(doesLibraryNotHaveID);
    }

    public List<string> SetDeck(List<string> newDeck) { 
        deckBase = newDeck; 

        ValidateDeck();

        return deckBase;
    }

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
                        card.gameObj.transform.SetParent(cardParent_Deck);
                        card.gameObj.transform.localPosition = Vector3.zero; // Reset card pos when moved to deck
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
            GameObject currCardObj = currCard.gameObj; 

            hand.Add(currCard);
            deck.RemoveAt(0);

            currCardObj.transform.SetParent(cardParent_Hand);
            currCardObj.SetActive(true);

            // Debug.Log("Draw " + (i + 1));

            // Card position in hand
            Vector2 newPos = new Vector2(0.0f - ((currCardObj.GetComponent<RectTransform>().rect.width / 2.0f) * (cardParent_Hand.childCount - 1)), 0.0f);

            currCardObj.transform.DOLocalMove(newPos, tweenDur);
            currCardObj.GetComponent<Card_Behaviour>().SetStartLocalPos(newPos);

            yield return World_AnimHandler.instance.WaitForSeconds(tweenDelay);
        }

        yield return World_AnimHandler.instance.WaitForSeconds(tweenDur - tweenDelay);

        UpdateNoOfCards_Hand();
    }

    private void ArrangeCardsInHand() {
        for (int i = 0; i < cardParent_Hand.childCount; i ++) {
            Transform card = cardParent_Hand.GetChild(i);
            Vector2 newPos = new Vector2(0.0f - ((card.GetComponent<RectTransform>().rect.width / 2.0f) * i), 0.0f); // Set card position in hand

            card.GetComponent<Card_Behaviour>().SetStartLocalPos(newPos);
            card.transform.DOLocalMove(newPos, 0.2f);
        }
    }

    public IEnumerator PlayCardToGY(ICard playedCard) {
        playedCard.gameObj.transform.SetParent(cardParent_Play); // Store the played card here temporarily

        ArrangeCardsInHand(); // Arrange cards in hand after moving played card out of hand

        player.EnergyHandler().DecreaseEnergy(playedCard.cost); // Reduce player's energy based on played card's cost

        yield return playedCard.Effect(); // Play card effect
        yield return MoveCardToGY(playedCard); // Move card to GY

        // World_AnimHandler.instance.isAnimating = false;
    }

    private IEnumerator MoveCardToGY(ICard playedCard) {
        Transform cardTrans = playedCard.gameObj.transform;

        float tweenDur = 0.2f;
        Vector3 endPoint = cardTrans.InverseTransformPoint(cardParent_GY.position) + cardTrans.localPosition + new Vector3(playedCard.behaviour.GetWidthOffset(), 0.0f, 0.0f);

        // Debug.Log("1: " + cardParent_GY.localPosition);
        // Debug.Log("2: " + cardTrans.localPosition);
        // Debug.Log("3: " + new Vector3(playedCard.behaviour.GetWidthOffset(), 0.0f, 0.0f));

        cardTrans.DOLocalMoveX(endPoint.x, tweenDur).SetEase(Ease.OutQuint);
        cardTrans.DOLocalMoveY(endPoint.y, tweenDur);
        
        yield return World_AnimHandler.instance.WaitForSeconds(tweenDur);

        cardTrans.SetParent(cardParent_GY);

        gy.Add(playedCard); // Add to GY list
        hand.RemoveAt(hand.FindIndex((card) => playedCard == card)); // Remove from hand list
        
        cardTrans.gameObject.SetActive(false); // Disable card after card effect
        cardTrans.localPosition = Vector3.zero; // Set card pos
        
        UpdateNoOfCards_GY();
        UpdateNoOfCards_Hand();
        UpdateNoOfCards_Deck(); // Also update deck card counter in case played card adds cards to deck

        World_AnimHandler.instance.isAnimating = false;
    }

    // Adds a card prefab to cardParent_Deck + Registers card to deck card list
    private void CreateCardObj_Deck(string id) {
        ICard cardPrefab = cardLibrary.GetCardByID(id);

        if (cardPrefab != null) {
            GameObject newCard = GameObject.Instantiate(cardPrefab.gameObj, cardParent_Deck);
            deck.Add(newCard.GetComponent<ICard>());
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
            UpdateNoOfCards_Deck();
        }
        else { Debug.LogError("Cannot add card ID '" + id + "' to deck!"); }
    }

    // Adds a card prefab to cardParent_GY + Registers card to gy card list
    private void CreateCardObj_GY(string id) {
        ICard cardPrefab = cardLibrary.GetCardByID(id);

        if (cardPrefab != null) {
            GameObject newCard = GameObject.Instantiate(cardPrefab.gameObj, cardParent_GY);
            gy.Add(newCard.GetComponent<ICard>());
            newCard.SetActive(false); // Cards are created disabled
            newCard.transform.localPosition = Vector3.zero;
            
            UpdateNoOfCards_GY();
        }
        else { Debug.LogError("Cannot add card ID '" + id + "' to GY!"); }
    }
    
    private void UpdateNoOfCards_Deck() { textDeck.text = deck.Count.ToString(); }
    private void UpdateNoOfCards_Hand() { textHand.text = hand.Count.ToString(); }
    private void UpdateNoOfCards_GY() { textGY.text = gy.Count.ToString(); }

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
        // Card parents must be set to true
        cardParent_GY.gameObject.SetActive(true);
        cardParent_Hand.gameObject.SetActive(true);
        cardParent_Deck.gameObject.SetActive(true);

        SetDeck(new List<string>() {
            "Test",
            "Test",
            "Test",
            "Test",
            "Test",
            "Test",
        }); 

        // Instantiate card objs into deck
        foreach (string id in deckBase) { CreateCardObj_Deck(id); }
    }
}