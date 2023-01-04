using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World_CardLister : MonoBehaviour {
    //! Handlers
    [SerializeField] private Player_Cards cardsHandler;

    // Misc. Stuff
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
            
        foreach (Transform card in scrollRect.content) { card.SetParent(cardPool); } // Move cards back into card pool
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void DisplayPile(ListType listType) {
        List<ICard> cardList;

        switch (listType) {
            case ListType.GY:
                cardList = new List<ICard>(cardsHandler.GetGY());
                title.text = "Discard Pile";
            break;
            case ListType.EXILE:
                cardList = new List<ICard>(cardsHandler.GetExile());
                title.text = "Exile Pile";
            break;
            default:
                cardList = new List<ICard>(cardsHandler.GetDeck());
                cardList.Sort((card1, card2) => card1.ID.CompareTo(card2.ID));
                title.text = "Deck Pile";
            break;
        }

        if (cardList.Count > 0) {
            // Generate enough cards in pool first
            if (cardList.Count > cardPool.childCount) { InstantiateCardsIntoPool(cardList.Count - cardPool.childCount); }

            // Local pos calculations for each card in selected pile
            // rowSize determines how many cards will be in a row. Might be useful if resolutions change
            //-------------------------------------------------------------------------------------------------------------------------------------------------
            Rect cardRect = cardList[0].GameObj.GetComponent<RectTransform>().rect;

            float cardPaddingX = 10.0f;
            float cardPaddingY = 10.0f;

            float contentWidth = scrollRect.content.rect.width;
            float contentHeight = scrollRect.content.rect.height;
            float cardWidth = cardRect.width + cardPaddingX;
            float cardHeight = cardRect.height + cardPaddingY;
            int rowSize = (int)(contentWidth / cardWidth);
            float startX = (-cardWidth * (float)rowSize / 2.0f) + (cardWidth / 2.0f);
            float startY = (contentHeight / 2.0f) + (cardHeight / 2.0f);

            int x = 0;
            int y = 0;
            
            foreach (ICard card in cardList) {
                Transform currCard = cardPool.GetChild(0);

                currCard.SetParent(scrollRect.content);
                currCard.localPosition = new Vector2(startX + (cardWidth * (float)x), (startY / 2.0f) + (cardHeight * (float)y));
                
                currCard.GetComponent<Card_UI>().Setup(card);

                // Once the loop reached the end of row, reset x and add 1 to y (Move to next column and start again)
                if (x + 1 >= rowSize - 1) {
                    x = 0;
                    y ++;
                }
                else { x ++; }
            }
            //-------------------------------------------------------------------------------------------------------------------------------------------------
            
            uiObj.SetActive(true);
        }
        else {
            Debug.Log("No cards found in " + listType);
        }
    }

    private void InstantiateCardsIntoPool(int no = 1) {
        for (int i = 0; i < no; i ++) { GameObject.Instantiate(cardPrefab, cardPool); }
    }

    void Awake() {
        if (!title) Debug.LogWarning("CardLister.cs is missing 'title'!");
        if (!cardsHandler) Debug.LogWarning("CardLister.cs is missing 'cardsHandler'!");

        uiObj.SetActive(false);
        cardPool.gameObject.SetActive(false);

        InstantiateCardsIntoPool(10); // Instantiate 10 cards into pool because most decks start with 10 cards
    }
}
