using System.Collections.Generic;
using UnityEngine;

public class World_BattleRewards : MonoBehaviour {
    // Handlers
    [SerializeField] private Player_CardLibrary cardLibrary;
    
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject uiParent;
    [SerializeField] private Transform cardPool, cardParent;

    // Button functions
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CloseList() {
        uiParent.SetActive(false);
            
        foreach (Transform card in cardParent) { card.SetParent(cardPool); } // Move cards back into card pool
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void DisplayRewards() {
        GenerateRewards_Cards();
        
        uiParent.SetActive(true);
    }

    private int Random1to100() { return Random.Range(1, 100); }

    private void GenerateRewards_Cards() {
        int noOfCards = 3;
        List<ICard> cardList = new List<ICard>();

        ICard GetRandomCard(List<ICard> list) { return list[Random.Range(0, list.Count - 1)]; }

        for (int i = 0; i < noOfCards; i ++) {
            // Perform a while loop to avoid duplicates in rewards
            while (cardList.Count == i) {
                int rand = Random1to100();
                ICard newReward;

                if (rand <= 5) { newReward = GetRandomCard(cardLibrary.GetLibrary_Rare()); }
                else if (rand <= 20) { newReward = GetRandomCard(cardLibrary.GetLibrary_Uncommon()); }
                else { newReward = GetRandomCard(cardLibrary.GetLibrary_Common()); }

                // Do not allow duplicate rewards
                if (!cardList.Contains(newReward)) { cardList.Add(newReward); }
            }
        }
        
        // Generate enough cards in pool first
        if (cardList.Count > cardPool.childCount) { InstantiateCardsIntoPool(cardList.Count - cardPool.childCount); }     

        // Adding cards to content
        // Positioning will be handled by content's GridLayoutGroup component     
        foreach (ICard card in cardList) {
            Transform currCard = cardPool.GetChild(0);

            currCard.SetParent(cardParent);
            currCard.GetComponent<Card_UI>().Setup(card);
        }
    }

    private void InstantiateCardsIntoPool(int no = 1) {
        for (int i = 0; i < no; i ++) {
            GameObject newCard = GameObject.Instantiate(cardPrefab, cardPool); 
            newCard.GetComponent<Card_Events>().enabled = false; // Disable the new card's Card_Events script
        }
    }

    void Awake() {
        //! Sanity checks
        if (!cardLibrary) Debug.LogError("World_BattleRewards does not have Player_CardLibrary.cs!"); 

        uiParent.SetActive(false);
        cardPool.gameObject.SetActive(false);

        InstantiateCardsIntoPool(5); // Instantiate 5 cards into card pool first
    }
}
