// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class World_Shop : MonoBehaviour {
    [Header("Handlers")]
    [SerializeField] private Player_CardLibrary cardLibrary;

    [Header("Objects")]
    [SerializeField] private GameObject uiParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardPool, cardParent;

    // Button functions
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CloseShop() {
        uiParent.SetActive(false);
            
        while (cardParent.childCount > 0) { cardParent.GetChild(0).SetParent(cardPool); } // Move cards back into card pool
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
        foreach (Card_Stats card in cardList) {
            Transform currCard = cardPool.GetChild(0);

            currCard.SetParent(cardParent);
            currCard.GetComponent<Card_Stats>().Copy(card);
        }
    }

    private void InstantiateCardsIntoPool(int no = 1) {
        for (int i = 0; i < no; i ++) {
            Card_Stats currCard = GameObject.Instantiate(cardPrefab, cardPool).GetComponent<Card_Stats>();

            currCard.isPlayable = false; // Set the card to unplayable

            // Add a click event to reward cards to add the card to deck and close the list
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { 
                if (!currCard.isPlayable && !World_AnimHandler.isAnimating) { 
                    CloseShop();
                }
            });

            currCard.EventHandler.triggers.Add(entry);
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        }
    }

    void Awake() {
        //! Sanity checks
        if (!cardLibrary) Debug.LogError("World_BattleRewards does not have Player_CardLibrary.cs!"); 

        CloseShop();
        cardPool.gameObject.SetActive(false);

        InstantiateCardsIntoPool(5); // Instantiate 5 cards into card pool first
    }
}
