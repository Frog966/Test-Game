using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// Handles movement as well as combat for player/enemy
public class World_Grid : MonoBehaviour {
    public static World_Grid instance;

    [SerializeField] private static int row = 6, col = 3;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Dictionary<IEntity, Vector2Int> entitiesPos = new Dictionary<IEntity, Vector2Int>(); // Contains all entities on grid and their position

    // A 2D array that goes from left to right, top to bottom;
    private World_GridNode[,] grid = new World_GridNode[row, col]; 

    // Getters
    public World_GridNode GetNode(Vector2Int vec2) { return isNodeInBounds(vec2) ? grid[vec2.x, vec2.y] : null; }
    public Vector2Int GetEntityGridPos(IEntity entity) { return entitiesPos.ContainsKey(entity) ? entitiesPos[entity] : new Vector2Int(); }

    // Movement
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set entities' grid position
    // Registers/updates the entitiesPos when an entity moves to a grid position
    // Has no restrictions other than requiring a valid grid pos
    public void SetGridPos(IEntity entity, Vector2Int vec2) {
        // Debug.Log("SetGridPos: " + entity + ", " + vec2);

        World_GridNode node = GetNode(vec2);

        // Sanity check in case node is out of bounds or does not have a World_GridNode component attached
        if (node) {
            if (!entitiesPos.ContainsKey(entity)) { entitiesPos.Add(entity, vec2); } // Register new entitiesPos key
            else { entitiesPos[entity] = vec2; } // Update entitiesPos

            // Move entity to new grid position
            // Use world position for accuracy
            entity.GameObj.transform.position = node.gameObject.transform.position;
        }
        // else {
        //     Debug.Log("Node is out of bounds or does not have component attached!");
        // }
    }

    // SetGridPos() with a "entities cannot move to a grid node with another entity on it" restriction
    public void MoveToPos(IEntity entity, Vector2Int vec2) {
        if (!entitiesPos.Values.ToList().Contains(vec2)) { SetGridPos(entity, vec2); }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    // Combat
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    // Overloaded functions to check if entity is on specfic grid pos
    public bool IsEntityHere(IEntity entity, Vector2Int pos) { return entitiesPos[entity] == pos; }
    public bool IsEntityHere(IEntity entity, List<Vector2Int> posList) { return posList.Contains(entitiesPos[entity]); }

    // Removes an entity from the library
    // Usually used when an enemy dies
    public void RemoveEntityFromGrid(IEntity entity) { entitiesPos.Remove(entity); }

    public IEnumerator TelegraphHere(List<Vector2Int> posList) {
        World_AnimHandler.Instance.isAnimating = true;

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.PlayAnim_Flicker(); 
            
            // Begin waiting at the last pos
            if (i >= posList.Count - 1) yield return World_AnimHandler.Instance.WaitForCurrentAnim(currNode.GetAnimator());
        }

        World_AnimHandler.Instance.isAnimating = false;
    }

    public IEnumerator FlashHere(List<Vector2Int> posList, float delay = 1.0f) {
        World_AnimHandler.Instance.isAnimating = true;

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.PlayAnim_Flash(); 
            
            // Begin waiting at the last pos
            if (i >= posList.Count - 1) yield return World_AnimHandler.Instance.WaitForSeconds(delay);
        }

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.StopAnim_Flash();
        }

        World_AnimHandler.Instance.isAnimating = false;
    }

    public void AttackHere(List<Faction> targetFactions, List<Vector2Int> posList, int totalDmg, int noOfHits = 1) {
        foreach (IEntity entity in GetEntitiesOnMap(targetFactions)) {
            if (IsEntityHere(entity, posList)) { ResolveHitsOnEntity(entity, totalDmg, noOfHits); }
        }
    }

    // Returns entities that are under certain factions according to targetFactions param
    // Param passes every faction as default
    private List<IEntity> GetEntitiesOnMap(List<Faction> targetFactions) { 
        if (targetFactions == null) targetFactions = System.Enum.GetValues(typeof(Faction)).Cast<Faction>().ToList();

        List<IEntity> targetEntities = entitiesPos.Keys.ToList();
        targetEntities.RemoveAll((key) => !targetFactions.Contains(key.Faction));

        return targetEntities;
    }

    private void ResolveHitsOnEntity(IEntity entity, int totalDmg, int noOfHits = 1) {
        for (int i = 0; i < noOfHits; i++) { entity.OnHit(); }

        entity.Health -= totalDmg;

        if (entity.Health <= 0) { 
            entity.OnDeath(); 
            RemoveEntityFromGrid(entity);
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    private bool isNodeInBounds(Vector2Int vec2) {
        if (vec2.x < 0 || vec2.x >= row || vec2.y < 0 || vec2.y >= col) { return false; }

        return true;
    }

    // Grid array must be initialized before Start()
    void Awake() {
        instance = this;

        int childCounter = 0;

        // Store/Init all grid nodes into array
        for(int x = 0; x < row; x++) {
            for(int y = 0; y < col; y++) {
                // Debug.Log("Node [" + x + ", " + y + "]: " + childCounter);
                
                grid[x, y] = nodeParent.GetChild(childCounter).GetComponent<World_GridNode>();
                grid[x, y].gameObject.name = "Node [" + x + ", " + y + "]"; // Set node name for easier debugging
                grid[x, y].SetIsPlayerControlled(x < 3); // Set right half of grid to enemy control

                childCounter++;
            }
        }
    }
}