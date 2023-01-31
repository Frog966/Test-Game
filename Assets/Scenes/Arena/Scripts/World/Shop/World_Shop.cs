// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class World_Shop : MonoBehaviour {
    [Header("Handlers")]
    [SerializeField] private Player player;
    [SerializeField] private Player_CardLibrary cardLibrary;
    [SerializeField] private World_Shop_NPC shopNPC;

    [Header("Objects")]
    [SerializeField] private GameObject uiParent;
    [SerializeField] private GameObject cardStockPrefab;
    [SerializeField] private Transform cardPool, cardParent;

    // Button functions
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CloseShop() {
        uiParent.SetActive(false);

        // Move cards back into card pool   
        while (cardParent.childCount > 0) {
            Transform currCard = cardParent.GetChild(0);

            currCard.gameObject.SetActive(true); // Reset shop card to active just in case it was bought
            currCard.localScale = Vector3.one; // Reset shop card scale because of hover anim

            currCard.SetParent(cardPool); 
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void OpenShop() {
        GenerateStock_Cards();
        
        uiParent.SetActive(true);
    }

    private void GenerateStock_Cards() {
        int noOfCards = 5;
        List<Card_Stats> cardList = new List<Card_Stats>();

        // Functions
        int Random1to100() { return Random.Range(1, 100); }
        Card_Stats GetRandomCard(List<Card_Stats> list) { return list[Random.Range(0, list.Count - 1)]; }

        // Generate random cards to select for rewards
        for (int i = 0; i < noOfCards; i ++) {
            // Perform a while loop to avoid duplicates in rewards
            while (cardList.Count == i) {
                int rand = Random1to100();
                Card_Stats newReward;

                if (rand <= 5) { newReward = GetRandomCard(cardLibrary.GetLibrary_Rare()); }
                else if (rand <= 20) { newReward = GetRandomCard(cardLibrary.GetLibrary_Uncommon()); }
                else { newReward = GetRandomCard(cardLibrary.GetLibrary_Common()); }

                // Do not allow duplicate rewards
                // if (!cardList.Contains(newReward)) { cardList.Add(newReward); }
                cardList.Add(newReward);
            }
        }
        
        // Generate enough cards in pool first
        if (cardList.Count > cardPool.childCount) { InstantiateCardsIntoPool(cardList.Count - cardPool.childCount); }

        // Adding cards to content
        // Positioning will be handled by content's GridLayoutGroup component
        // foreach (Card_Stats card in cardList) {
        for (int i = 0; i < cardList.Count; i ++) {
            Card_Stats card = cardList[i];
            Transform currCard = cardPool.GetChild(0);

            currCard.SetParent(cardParent);
            currCard.GetComponent<World_Shop_Card>().Setup(card);

            float cardOffset = card.UIHandler.GetWidth() + 20.0f;
            float startPoint = -(cardOffset * (cardList.Count - 1.0f) / 2.0f);

            currCard.localPosition = new Vector2(startPoint + cardOffset * i, 0.0f);
        }
    }

    private void InstantiateCardsIntoPool(int no = 1) {
        for (int i = 0; i < no; i ++) {
            World_Shop_Card currShopCard = GameObject.Instantiate(cardStockPrefab, cardPool).GetComponent<World_Shop_Card>();
            Card_Stats currCardScript = currShopCard.GetCard();

            // Add a click event to reward cards to add the card to deck and close the list
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { 
                if (!currCardScript.isPlayable && !AnimHandler.isAnimating) { 
                    if (player.GetBits() >= currShopCard.GetPrice()) {
                        Player_Cards.Deck.AddCardToDeck(currCardScript.ID);
                        
                        // Reduce player bits according to card price
                        player.SetBits(player.GetBits() - currShopCard.GetPrice());

                        // Disable card after being bought
                        currShopCard.gameObject.SetActive(false);
                    }
                    else {
                        shopNPC.PopUp_NotEnoughMoney();
                    }
                }
            });

            currCardScript.EventHandler.triggers.Add(entry);
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        }
    }

    void Awake() {
        //! Sanity checks
        if (!player) { Debug.LogError("World_Shop does not have Player.cs!"); }
        if (!cardLibrary) {
            if (player) { cardLibrary = player.GetComponent<Player_CardLibrary>(); }
            else { Debug.LogError("World_Shop does not have Player_CardLibrary.cs!"); }
        }
        if (!shopNPC) { Debug.LogError("World_Shop does not have World_Shop_NPC.cs!"); }

        CloseShop();
        cardPool.gameObject.SetActive(false);

        while (cardPool.childCount > 0) { DestroyImmediate(cardPool.GetChild(0).gameObject); } // Destroy all objects in card pool

        InstantiateCardsIntoPool(5); // Instantiate 5 cards into card pool first
    }
}
