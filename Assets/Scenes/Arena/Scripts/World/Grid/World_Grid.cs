using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// Handles movement as well as combat for player/enemy
public class World_Grid : MonoBehaviour {
    public static World_Grid instance;
    private World world;

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

    // Returns a valid pos list that is relative to the origin point
    // relativePosList MUST be values that are relative to origin point
    // eg. origin: (0, 0), relativePosList: [(-1, 0)] means the post list will be the left of origin
    // Set includeOrigin to false if you don't want to include the origin point
    public List<Vector2Int> ReturnRelativePosList(Vector2Int origin, List<Vector2Int> relativePosList, bool includeOrigin = true) {
        List<Vector2Int> validPosList = new List<Vector2Int>();

        // Calculate and add any pos that are within the grid
        foreach (Vector2Int v2 in relativePosList) {
            int newX = v2[0] + origin[0];
            int newY = v2[1] + origin[1];

            if (newX >= 0 && newX < row && newY >= 0 && newY < col) { validPosList.Add(new Vector2Int(newX, newY)); }
        }

        if (includeOrigin) { validPosList.Add(origin); }
        else { validPosList.RemoveAll((v2) => v2 == origin); } // Remove any points that are equal to origin in case someone added it accidentally

        return ReturnDistinctPosList(validPosList); // Remove any duplicates 
    }

    public IEnumerator TelegraphHere(List<Vector2Int> posList) {
        World_AnimHandler.instance.isAnimating = true;
        
        ReturnDistinctPosList(posList); // Remove any duplicates 

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.PlayAnim_Flicker(); 
            
            // Begin waiting at the last pos
            if (i >= posList.Count - 1) yield return World_AnimHandler.instance.WaitForCurrentAnim(currNode.GetAnimator());
        }

        World_AnimHandler.instance.isAnimating = false;
    }

    public IEnumerator FlashHere(List<Vector2Int> posList, float delay = 0.25f) {
        World_AnimHandler.instance.isAnimating = true;
        
        ReturnDistinctPosList(posList); // Remove any duplicates 

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.PlayAnim_Flash(); 
            
            // Begin waiting at the last pos
            if (i >= posList.Count - 1) yield return World_AnimHandler.instance.WaitForSeconds(delay);
        }

        for (int i = 0; i < posList.Count; i++) { 
            World_GridNode currNode = grid[posList[i][0], posList[i][1]];

            currNode.StopAnim_Flash();
        }

        World_AnimHandler.instance.isAnimating = false;
    }

    public void HitHere(Faction attackerFaction, List<Vector2Int> posList, int dmg) {
        ReturnDistinctPosList(posList); // Remove any duplicates 

        foreach (IEntity entity in GetEntitiesOnMap(attackerFaction)) {
            if (IsEntityHere(entity, posList)) { ResolveHitsOnEntity(entity, dmg); }
        }
    }

    // Returns a pos list without any duplicates
    private List<Vector2Int> ReturnDistinctPosList(List<Vector2Int> posList) { return posList.Distinct().ToList(); }

    // Removes an entity from the library
    // Usually used when an enemy dies
    private void RemoveEntityFromGrid(IEntity entity) { entitiesPos.Remove(entity); }

    // Returns entities that are not the attacker's faction
    private List<IEntity> GetEntitiesOnMap(Faction attackerFaction) {
        List<Faction> targetFactions = System.Enum.GetValues(typeof(Faction)).Cast<Faction>().ToList(); // Remove attacker faction from faction list
        targetFactions.Remove(attackerFaction);

        return entitiesPos.Keys.ToList().Where((entity) => targetFactions.Contains(entity.Faction)).ToList(); // Return entities that belong to the remain factions
    }

    private void ResolveHitsOnEntity(IEntity entity, int damage) {
        entity.OnHit(damage); 

        // Entity dies here
        if (entity.Health <= 0) { 
            entity.OnDeath(); 
            RemoveEntityFromGrid(entity);
            world.TurnsHandler().RemoveAllTurnsByEntity(entity);
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

    void Start() {
        // Sanity checks
        world = this.gameObject.GetComponent<World>();
    }
}