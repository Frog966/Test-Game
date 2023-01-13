using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum RewardType {
    BITS,
    CARDS,
}

public class World_BattleRewards : MonoBehaviour {
    [Header("Handlers")]
    [SerializeField] private Player player;
    [SerializeField] private World_Map mapHandler;
    [SerializeField] private Player_CardLibrary cardLibrary;
    
    [Header("Objects")]
    [SerializeField] private GameObject uiParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardUI, cardPool, cardParent;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonPool, buttonParent;

    // Button functions
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CloseRewards() {
        uiParent.SetActive(false);
        
        while (buttonParent.childCount > 0) { buttonParent.GetChild(0).SetParent(buttonPool); } // Move buttons back into button pool
        
        // Move cards back into card pool
        while (cardParent.childCount > 0) { 
            Transform currCard = cardParent.GetChild(0);

            currCard.localScale = Vector3.one; // Reset shop card scale because of hover anim
            currCard.SetParent(cardPool); 
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void DisplayRewards() {
        GenerateRewards_Coins();
        GenerateRewards_Cards();
        
        uiParent.SetActive(true);
    }

    private int Random1to100() { return Random.Range(1, 100); }

    private void GenerateRewards_Coins() {
        SetupButton(RewardType.BITS);
    }

    private void GenerateRewards_Cards() {
        SetupButton(RewardType.CARDS);

        int noOfCards = 3;
        List<Card_Stats> cardList = new List<Card_Stats>();

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
                if (!cardList.Contains(newReward)) { cardList.Add(newReward); }
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
                    Player_Cards.Deck.AddCardToDeck(currCard.ID);
                    
                    cardUI.gameObject.SetActive(false);
                }
            });

            currCard.EventHandler.triggers.Add(entry);
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        }
    }

    private void SetupButton(RewardType rewardType) {
        Transform button = buttonPool.GetChild(0);
        World_BattleRewards_Button buttonScript = button.GetComponent<World_BattleRewards_Button>();

        int coinReward = Random.Range(30, 50);

        button.SetParent(buttonParent);
        buttonScript.Setup(rewardType, coinReward);

        switch(rewardType) {
            case RewardType.BITS: 
                buttonScript.GetButton().onClick.AddListener(() => {
                    Debug.Log("Player receives " + coinReward + " coins!");

                    player.SetBits(player.GetBits() + coinReward);
                    button.SetParent(buttonPool);
                });
            break;
            default: 
                buttonScript.GetButton().onClick.AddListener(() => {
                    Debug.Log("Player receives a new card!");

                    cardUI.gameObject.SetActive(true);
                    button.SetParent(buttonPool);
                });
            break;
        }
    }

    void Awake() {
        //! Sanity checks
        if (!player) { Debug.LogError("World_BattleRewards does not have Player.cs!"); }
        if (!mapHandler) mapHandler = this.GetComponent<World_Map>(); 
        if (!cardLibrary) Debug.LogError("World_BattleRewards does not have Player_CardLibrary.cs!"); 

        CloseRewards();
        cardUI.gameObject.SetActive(false);
        cardPool.gameObject.SetActive(false);
        buttonPool.gameObject.SetActive(false);

        InstantiateCardsIntoPool(5); // Instantiate 5 cards into card pool first

        while (buttonPool.childCount > 0) { DestroyImmediate(buttonPool.GetChild(0).gameObject); } // Destroy all objects in button pool
        while (buttonParent.childCount > 0) { DestroyImmediate(buttonParent.GetChild(0).gameObject); } // Destroy all objects in button parent

        while (buttonPool.childCount < 3) { GameObject.Instantiate(buttonPrefab, buttonPool); } // Instantiate 3 buttons into button pool
    }
}