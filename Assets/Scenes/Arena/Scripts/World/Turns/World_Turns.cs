using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// In charge of turn creation, order and execution
public class World_Turns : MonoBehaviour {
    private World world;

    [SerializeField] private GameObject turnPrefab;
    [SerializeField] private Transform turnParent, turnPool;

    [SerializeField] private List<World_Turn> turnList = new List<World_Turn>();
    private List<World_Turn> turnListNext = new List<World_Turn>();

    // Setup the encounter
    public void SetupEncounter(World_MapNode currMapNode) {
        // Reset player pos
        World_Grid.instance.SetGridPos(world.Player(), Vector2Int.one);

        // Instantiate and set enemies' pos
        foreach (Game.Map.EncounterEnemyDetails details in currMapNode.GetEncounter()) {
            GameObject newEnemyObj = GameObject.Instantiate(details.enemy.GameObj, world.EnemyParent());
            World_Grid.instance.SetGridPos(newEnemyObj.GetComponent<IEnemy>(), details.gridPos);
        }

        // Set up first turn for player and enemy
        // Player always first
        CreateTurn(world.Player());
        
        // Create turns for enemies
        foreach (Transform child in world.EnemyParent()) {
            CreateTurn(child.GetComponent<IEnemy>());
        }

        ArrangeTurnObjs();

        StartCoroutine(StartTurn()); // Start 1st turn
    }

    // Start current turn
    // If enemy, perform turn's task list then end the turn. If player, turn will continue until player ends turn manually
    // Perform certain buff/debuff updates here
    public IEnumerator StartTurn() {
        World_Turn currTurn = turnList[0];

        // Only end the turn if currTurn's owner is not a Player script
        if (!(currTurn.GetOwner() is Player)) { 
            yield return currTurn.Execute();

            EndTurn(); 
        }
        else {
            world.Player().StartTurn();
        }
    }

    // Remove turn obj from turnParent as well as update turnList
    public void RemoveTurn(int i = 0) {
        // Moving selected turn into turnPool then rearrange the turnParent
        turnList[i].transform.SetParent(turnPool);
        turnList.RemoveAt(i);

        ArrangeTurnObjs();
    }

    // Remove all turns that belong to entity param
    public void RemoveAllTurnsByEntity(IEntity entity) {
        List<World_Turn> removableTurns = turnList.FindAll((turn) => turn.GetOwner() == entity); // Find all turns owned by entity
        foreach (World_Turn turn in removableTurns) { turn.transform.SetParent(turnPool); } // Move turn obj back to pool

        turnList.RemoveAll((turn) => removableTurns.Contains(turn)); // Remove said turns from turn list
    }

    // End current turn
    // Perform certain buff/debuff updates here
    public void EndTurn() {
        CreateTurn(turnList[0].GetOwner()); // Recreate the current turn. If enemy, possibly progress it's pattern
        RemoveTurn();
        
        // Only start turn if there's a player
        if (turnList.Find((turn) => turn.GetOwner() is Player)) {
            StartCoroutine(StartTurn()); // Start the next turn which is the new current
        }
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
    private void CreateTurn(IEntity owner) {
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

        newTurn.Setup(owner, !(owner is Player) ? owner.GameObj.GetComponent<IEnemy>().ReturnCurrentTurn() : null); // If enemy, pass task list. Else, pass null
        newTurn.transform.SetParent(turnParent); // Place newTurn into turnParent
        // newTurn.transform.SetAsLastSibling();
        
        turnList.Add(newTurn); // Add newTurn to turnList
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
    }
}