using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World_CardLister : MonoBehaviour {
    //! Handlers
    [Header("Handlers")]
    [SerializeField] private Player_Cards cardsHandler;

    // Misc. Stuff
    [Header("Misc. Stuff")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject uiObj;
    [SerializeField] private Text title;
    [SerializeField] private Transform cardPool;
    [SerializeField] private ScrollRect scrollRect;

    private enum ListType {
        DECK,
        GY,
        EXILE
    }

    // Button functions
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void DisplayPile_GY() { DisplayPile(ListType.GY); }
    public void DisplayPile_Deck() { DisplayPile(ListType.DECK); }
    public void DisplayPile_Exile() { DisplayPile(ListType.EXILE); }

    public void CloseList() {
        uiObj.SetActive(false);
        
        while (scrollRect.content.childCount > 0) { scrollRect.content.GetChild(0).SetParent(cardPool); } // Move cards back into card pool
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void DisplayPile(ListType listType) {
        // Do not display card list when an animation is playing as player could be using a card which would affect the card lists
        if (!AnimHandler.isAnimating) {            
            // Determine which card list we're getting from Player_Cards.cs
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            List<Card_Stats> cardList;

            switch (listType) {
                case ListType.GY:
                    cardList = new List<Card_Stats>(cardsHandler.GetGY());
                    title.text = "Discard Pile";
                break;
                case ListType.EXILE:
                    cardList = new List<Card_Stats>(cardsHandler.GetExile());
                    title.text = "Exile Pile";
                break;
                default:
                    cardList = new List<Card_Stats>(cardsHandler.GetDeck());
                    cardList.Sort((card1, card2) => card1.ID.CompareTo(card2.ID)); // Only deck list is sorted alphabetically to avoid people learning deck order
                    title.text = "Deck Pile";
                break;
            }
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------

            if (cardList.Count > 0) {
                // Generate enough cards in pool first
                if (cardList.Count > cardPool.childCount) { InstantiateCardsIntoPool(cardList.Count - cardPool.childCount); }     

                // Adding cards to content
                // Positioning will be handled by content's GridLayoutGroup component     
                foreach (Card_Stats card in cardList) {
                    Transform currCard = cardPool.GetChild(0);

                    currCard.SetParent(scrollRect.content);
                    currCard.GetComponent<Card_Stats>().Copy(card);
                }
                
                uiObj.SetActive(true);
                scrollRect.verticalNormalizedPosition = 1.0f;
            }
            else {
                Debug.Log("No cards found in " + listType);
            }
        }
    }

    private void InstantiateCardsIntoPool(int no = 1) {
        for (int i = 0; i < no; i ++) {
            GameObject newCard = GameObject.Instantiate(cardPrefab, cardPool); 
            newCard.GetComponent<Card_Stats>().isPlayable = false; // Set the card to unplayable
        }
    }

    void Awake() {
        if (!title) Debug.LogWarning("CardLister.cs is missing 'title'!");
        if (!cardsHandler) Debug.LogWarning("CardLister.cs is missing 'cardsHandler'!");

        uiObj.SetActive(false);
        cardPool.gameObject.SetActive(false);

        InstantiateCardsIntoPool(10); // Instantiate 10 cards into pool because most decks start with 10 cards
    }
}
