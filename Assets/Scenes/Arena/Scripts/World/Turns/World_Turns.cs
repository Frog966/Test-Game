using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// In charge of turn creation, order and execution
public class World_Turns : MonoBehaviour {
    // Handlers
    [SerializeField] private Player player;
    [SerializeField] private World_Map mapHandler;
    [SerializeField] private World_Shop shopHandler;
    [SerializeField] private Player_Cards cardHandler;

    [SerializeField] private GameObject nextMapNodeButton;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform enemyTrash; // The transform to pool enemies to be destroyed

    [Header("Turn Stuff")]
    [SerializeField] private GameObject turnPrefab;
    [SerializeField] private Transform turnParent, turnPool;

    private List<World_Turn> turnList = new List<World_Turn>();
    // private List<World_Turn> turnListNext = new List<World_Turn>();

    // Setup the encounter
    public void StartEncounter(World_MapNode currMapNode) {
        AnimHandler.isAnimating = true;

        // Set up units
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        World_Grid.Movement.SetGridPos(player.GetEntity(), Vector2Int.one); // Reset player pos

        ClearEnemyParent(); // Destroy every GO in enemyParent
        cardHandler.ResetCards(); // Reset player cards

        if (mapHandler.GetCurrMapNode().GetNodeType() == Game.Map.NodeType.SHOP) {
            shopHandler.OpenShop();
        }
        else {
            // Instantiate and set enemies' pos
            foreach (Game.Map.EncounterEnemyDetails details in currMapNode.GetEncounter()) {
                GameObject newEnemyObj = GameObject.Instantiate(details.enemy.Entity.gameObject, enemyParent);
                World_Grid.Movement.SetGridPos(newEnemyObj.GetComponent<Entity>(), details.gridPos);
            }

            // Set up turns
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            // Set up first turn for player and enemy
            // Player always first
            CreateTurn(player.GetEntity());
            
            // Create turns for enemies
            foreach (Transform child in enemyParent) { CreateTurn(child.GetComponent<Entity>()); }

            ArrangeTurnObjs();
            //-----------------------------------------------------------------------------------------------------------------------------------------------

            cardHandler.Shuffle(); // Shuffle player's deck before start of encounter
            cardHandler.CardUI_Enter();
            
            StartCoroutine(StartTurn()); // Start 1st turn
        }
    }

    // End the encounter if possible
    public void HasEncounterEnded() {
        // If all enemies died
        if (!(turnList.Find((turn) => turn.GetOwner() != player.GetEntity()))) {
            Debug.Log("Player Won!");

            foreach (Transform enemy in enemyParent) { Destroy(enemy.gameObject); } // Destroy all enemies if player won
            foreach (World_Turn turn in turnList) { turn.transform.SetParent(turnPool); } // Move all turn obj back to pool

            StartCoroutine(cardHandler.CardUI_Exit());

            nextMapNodeButton.SetActive(true);
        }
        else { // If player died
            Debug.Log("Player Lost!");
        }
    }

    // Remove all turns that belong to entity param
    public void RemoveAllTurnsByEntity(Entity entity) {
        List<World_Turn> removableTurns = turnList.FindAll((turn) => turn.GetOwner() == entity); // Find all turns owned by entity
        foreach (World_Turn turn in removableTurns) { turn.transform.SetParent(turnPool); } // Move turn obj back to pool

        turnList.RemoveAll((turn) => removableTurns.Contains(turn)); // Remove said turns from turn list
    }

    // Used as button function
    public void DisableNextMapNodeButton() { nextMapNodeButton.SetActive(false); }

    // End current turn
    // Perform certain buff/debuff updates here
    public IEnumerator EndTurn() {
        // Only start turn if there's a player and enemy
        if (turnList.Find((turn) => turn.GetOwner() == player.GetEntity()) && turnList.Find((turn) => turn.GetOwner() != player.GetEntity())) {
            Entity currEntity = turnList[0].GetOwner();

            yield return currEntity.EndTurn(); // Perform actions that all entities must do at end of turn

            CreateTurn(turnList[0].GetOwner()); // Recreate the current turn. If enemy, progress it's pattern if needed
            RemoveTurn();

            yield return StartTurn(); // Start the next turn which is the new current
        }
        else { AnimHandler.isAnimating = false; } // Stop animating if 1 faction is gone
    }

    // Start current turn
    // If enemy, perform turn's task list then end the turn. If player, turn will continue until player ends turn manually
    // Perform certain buff/debuff updates here
    private IEnumerator StartTurn() {
        World_Turn currTurn = turnList[0];
        Entity currEntity = currTurn.GetOwner();

        yield return currEntity.StartTurn(); // Perform actions that all entities must do at start of turn

        // Only end the turn if currTurn's owner is not a Player script
        if (currEntity != player.GetEntity()) {
            yield return currTurn.Execute(); // Execute the non-player entity's behaviour
            yield return EndTurn(); // End non-player entity's turn
        }
        else { yield return player.StartTurn(); } //! The game will not stop animating until the player's turn
    }

    // Remove turn obj from turnParent as well as update turnList
    private void RemoveTurn(int i = 0) {
        // Moving selected turn into turnPool then rearrange the turnParent
        turnList[i].transform.SetParent(turnPool);
        turnList.RemoveAt(i);

        ArrangeTurnObjs();
    }

    // Instantiates a turn prefab to turnPool and returns the new GO's World_Turn component
    private World_Turn InstantiateTurn() {
        return Instantiate(turnPrefab, turnPool).GetComponent<World_Turn>();
    }

    private void ArrangeTurnObjs() {
        for(int i = 0; i < turnParent.childCount; i++) {
            RectTransform currChild = turnParent.GetChild(i).GetComponent<RectTransform>();

            currChild.localPosition = new Vector2(currChild.rect.width * i, 0);
        }
    }

    // Recycles a turn prefab from turnPool and passes it to turnParent
    // If no prefabs are available, instantiate a new one in turnPool then passes it to turnParent
    private void CreateTurn(Entity owner) {
        World_Turn newTurn = null;

        if (turnPool.childCount < 1) {
            newTurn = InstantiateTurn();
        }
        else {
            // A sanity check in case the first child does not have a World_Turn component
            while (turnPool.childCount > 0) {
                World_Turn currChild = turnPool.GetChild(0).GetComponent<World_Turn>();
                
                // If currChild does not have World_Turn component, destroy it
                // Else, newTurn is currChild and then exit the loop
                if (!currChild) {
                    Destroy(currChild.gameObject);
                }
                else {
                    newTurn = currChild;
                    break;
                }
            }

            // In the event the only child in turnPool is invalid
            if (turnPool.childCount < 1) {
                newTurn = InstantiateTurn();
            }
        }

        // Debug.Log("Test: " + owner + ", " + (owner is IEnemy));

        newTurn.Setup(owner, owner.GetComponent<IEnemy>() != null ? owner.GetComponent<IEnemy>().ReturnCurrentTurn() : null); // If enemy, pass task list. Else, pass null
        newTurn.transform.SetParent(turnParent); // Place newTurn into turnParent
        
        turnList.Add(newTurn); // Add newTurn to turnList
    }

    // Destroy every enemy in enemyParent
    private void ClearEnemyParent() {
        foreach (Transform child in enemyParent) { 
            child.SetParent(enemyTrash);
            Destroy(child.gameObject); 
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!mapHandler) mapHandler = this.GetComponent<World_Map>();
        if (!shopHandler) shopHandler = this.GetComponent<World_Shop>();
        if (!player) Debug.LogError("World_Turns does not have Player.cs!"); 
        if (!cardHandler) Debug.LogError("World_Turns does not have Player_Cards.cs!"); 

        turnPool.gameObject.SetActive(false);
        turnParent.gameObject.SetActive(true);
        nextMapNodeButton.SetActive(false);

        ClearEnemyParent(); // Destroy every GO in enemyParent
        
        // Destroy any GOs that do not have World_Turn component in turnPool
        foreach (Transform child in turnPool) {
            if (!child.GetComponent<World_Turn>()) {
                child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
                Destroy(child.gameObject);
            }
        }

        // Move any turns in turnParent to turnPool
        // Destroy any GOs that do not have World_Turn component
        foreach (Transform child in turnParent) {
            if (child.GetComponent<World_Turn>()) {
                child.SetParent(turnPool);
            }
            else {
                child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
                Destroy(child.gameObject);
            }
        }

        // Instantiate 5 turn objects in advance in turnPool. Includes any turn objects that were moved from turnParent just now
        while (turnPool.childCount < 5) {
            World_Turn newTurn = InstantiateTurn();
        }
    }
}