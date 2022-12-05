using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// In charge of turn creation, order and execution
public class World_Turns : MonoBehaviour {
    private World world;

    public GameObject turnPrefab;
    public Transform turnParent, turnPool;

    private List<World_Turn> turnList = new List<World_Turn>();
    private List<World_Turn> turnListNext = new List<World_Turn>();

    // Instantiates a turn prefab to parent and returns the new GO's World_Turn component
    // Default parent is turnPool
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
    private void CreateTurn(Entity owner, List<Task> taskList = null) {
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

        newTurn.Setup(owner, taskList);
        turnList.Add(newTurn); // Add newTurn to turnList
        newTurn.transform.SetParent(turnParent); // Place newTurn into turnParent
        ArrangeTurnObjs();
    }

    // Generate turns first
    public void SetupTurns() {
        // Set up first turn for player and enemy
        // Player always first
        CreateTurn(world.player);
        
        // Create turns for enemies
        foreach (Transform child in world.enemyParent) {

        }
    }

    // Start current turn
    // If enemy, perform turn's task list then end the turn. If player, turn will continue until player ends turn manually
    // Perform certain buff/debuff updates here
    public void StartTurn() {
        World_Turn currTurn = turnList[0];
        
        // Debug.Log("StartTurn 1");

        // Only end the turn if currTurn's owner is not a Player script
        if (!(currTurn.GetOwner() is Player)) {
            // Debug.Log("StartTurn 2");

            EndTurn();
        }
        else {
            // Best to use a function like this instead of just calling all the functions inside this function
            world.player.StartTurn();
        }
    }

    // Remove current turn obj from turnParent as well as update turnList
    public void RemoveTurn(int i = 0) {
        // Moving selected turn into turnPool then rearrange the turnParent
        World_Turn selectedTurn = turnList[i];
        selectedTurn.transform.SetParent(turnPool);
        ArrangeTurnObjs();

        turnList.RemoveAt(i);
    }

    // End current turn
    // Perform certain buff/debuff updates here
    public void EndTurn() {
        RemoveTurn();
        
        // StartTurn(); // Start the next turn which is the new current 
    }

    void Awake() {
        turnPool.gameObject.SetActive(false);
        turnParent.gameObject.SetActive(true);
        
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

    void Start() {
        // Sanity checks
        world = this.gameObject.GetComponent<World>();

        SetupTurns();
        StartTurn();
    }
}