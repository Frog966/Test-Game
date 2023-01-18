using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// This script contains the World_Grid static class
// The MonoBehaviour class is simply to initialize some things required by World_Grid
public class World_Grid : MonoBehaviour {
    [SerializeField] private Transform gridNodeParent;
    
    private static World_Turns turnsHandler;
    
    // Singleton instance
    // Set to private to quietly check for only one instance of this class to delete any extra World_Grid.cs scripts in the scene
    private static World_Grid Inst { get; set; }

    private static int row = 6, col = 3;
    private static World_GridNode[,] grid = new World_GridNode[row, col]; // A 2D array that goes from left to right, top to bottom;
    private static Dictionary<Entity, Vector2Int> entitiesPos = new Dictionary<Entity, Vector2Int>(); // Contains all entities on grid and their position

    // Getters
    public static int GetRow() { return row; }
    public static int GetCol() { return col; }
    public static World_GridNode GetNode(int x, int y) { return isCoorInGrid(x, y) ? grid[x, y] : null; }
    public static World_GridNode GetNode(Vector2Int vec2) { return isCoorInGrid(vec2) ? grid[vec2.x, vec2.y] : null; }
    public static Vector2Int GetEntityGridPos(Entity entity) { return entitiesPos.ContainsKey(entity) ? entitiesPos[entity] : new Vector2Int(); }

    // Overloaded functions to check if entity is on specfic grid pos
    public static bool IsEntityHere(Entity entity, Vector2Int pos) { return entitiesPos[entity] == pos; }
    public static bool IsEntityHere(Entity entity, int posX, int posY) { return entitiesPos[entity] == new Vector2Int(posX, posY); }
    public static bool IsEntityHere(Entity entity, List<Vector2Int> posList) { return posList.Contains(entitiesPos[entity]); }
    
    // Overloaded functions to check if there is an entity on specfic grid pos
    public static bool IsThereAnEntity(Vector2Int here) { return entitiesPos.Values.ToList().Contains(here); }
    public static bool IsThereAnEntity(int hereX, int hereY) { return IsThereAnEntity(new Vector2Int(hereX, hereY)); }

    // Setters
    public static World_GridNode RecordNode(World_GridNode node, int x, int y) { 
        grid[x, y] = node; 

        return grid[x, y]; // Returns recorded node
    }
    
    private static bool isCoorInGrid(int x, int y) { return (x >= 0 && x < row && y >= 0 && y < col); }
    private static bool isCoorInGrid(Vector2Int vec2) { return isCoorInGrid(vec2.x, vec2.y); }
    
    // A built-in container to hold all movement-related functions for World_Grid
    public static class Movement {
        // Set entities' grid position
        // Registers/updates the entitiesPos when an entity moves to a grid position
        // Has no restrictions other than requiring a valid grid pos
        public static void SetGridPos(Entity entity, Vector2Int vec2) {
            // Debug.Log("SetGridPos: " + entity + ", " + vec2);

            World_GridNode targetNode = GetNode(vec2);

            // Sanity check in case node is out of bounds or does not have a World_GridNode component attached
            if (targetNode) {
                if (!entitiesPos.ContainsKey(entity)) { entitiesPos.Add(entity, vec2); } // Register new entitiesPos key
                else { entitiesPos[entity] = vec2; } // Update entitiesPos

                // Move entity to new grid position
                // Use world position for accuracy
                entity.transform.position = targetNode.gameObject.transform.position;
            }
            else {
                Debug.LogWarning("Node is out of bounds or does not have component attached!");
            }
        }

        // SetGridPos() with a "entities cannot move to a grid node with another entity on it" restriction
        public static void MoveToPos(Entity entity, Vector2Int gridCoor) {
            if (!entitiesPos.Values.ToList().Contains(gridCoor)) { SetGridPos(entity, gridCoor); }
        }

        // Some overloaded functions in case you just want to pass 2 ints instead
        public static void SetGridPos(Entity entity, int x, int y) { SetGridPos(entity, new Vector2Int(x, y)); }
        public static void MoveToPos(Entity entity, int x, int y) { MoveToPos(entity, new Vector2Int(x, y)); }
    }

    public static class Combat {
        // Returns a pos list without any duplicates
        public static List<Vector2Int> ReturnDistinctPosList(List<Vector2Int> posList) { return posList.Distinct().ToList(); }

        public static IEnumerator TelegraphHere(List<Vector2Int> posList) {
            World_AnimHandler.isAnimating = true;
            
            ReturnDistinctPosList(posList); // Remove any duplicates 

            for (int i = 0; i < posList.Count; i++) { 
                World_GridNode currNode = grid[posList[i][0], posList[i][1]];

                currNode.PlayAnim_Flicker(); 
                
                // Begin waiting at the last pos
                if (i >= posList.Count - 1) yield return World_AnimHandler.WaitForCurrentAnim(currNode.GetAnimator());
            }

            World_AnimHandler.isAnimating = false;
        }

        public static IEnumerator FlashHere(List<Vector2Int> posList, float delay = 0.25f) {
            World_AnimHandler.isAnimating = true;
            
            ReturnDistinctPosList(posList); // Remove any duplicates 

            for (int i = 0; i < posList.Count; i++) { 
                World_GridNode currNode = grid[posList[i][0], posList[i][1]];

                currNode.PlayAnim_Flash(); 
                
                // Begin waiting at the last pos
                if (i >= posList.Count - 1) yield return World_AnimHandler.WaitForSeconds(delay);
            }

            for (int i = 0; i < posList.Count; i++) { 
                World_GridNode currNode = grid[posList[i][0], posList[i][1]];

                currNode.StopAnim_Flash();
            }

            World_AnimHandler.isAnimating = false;
        }

        // Returns list of hit entities
        public static List<Entity> HitHere(Faction attackerFaction, List<Vector2Int> posList, int dmg) {
            ReturnDistinctPosList(posList); // Remove any duplicates 

            List<Entity> hitEntities = new List<Entity>(); // Hold all entities that are hit

            foreach (Entity entity in GetEntitiesOnMap(attackerFaction)) {
                if (IsEntityHere(entity, posList)) { 
                    entity.OnHit(dmg); 

                    // Entity dies here
                    if (entity.GetHealth() <= 0) {
                        Debug.Log(entity.gameObject.name + " has died!");

                        entity.OnDeath(); // The entity disables itself here
                        
                        entitiesPos.Remove(entity); // Remove entity from the library
                        turnsHandler.RemoveAllTurnsByEntity(entity); // Remove all turns by said entity

                        turnsHandler.HasEncounterEnded(); // Check if encounter has ended
                    }
                    
                    hitEntities.Add(entity);
                }
            }

            return hitEntities;
        }

        // Returns a valid pos list that is relative to the origin point
        // relativePosList MUST be values that are relative to origin point
        // eg. origin: (0, 0), relativePosList: [(-1, 0)] means the post list will be the left of origin
        // If you add origin, it'll be included in the returned list
        public static List<Vector2Int> ReturnRelativePosList(Vector2Int origin, List<Vector2Int> relativePosList, bool includeOrigin = false) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            // Calculate and add any pos that are within the grid
            foreach (Vector2Int v2 in relativePosList) {
                Vector2Int newCoor = new Vector2Int(v2[0] + origin[0], v2[1] + origin[1]);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
            }

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight line to the left of origin
        public static List<Vector2Int> ReturnPosList_Left(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            for (int i = origin.x; i > 0; i--) { 
                Vector2Int newCoor = new Vector2Int(i, origin.y);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
                if (!isPiercing && i != origin.x && IsThereAnEntity(newCoor)) { break; } // Stop moving left if not piercing and entity is found
            }

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight line to the right of origin
        public static List<Vector2Int> ReturnPosList_Right(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            for (int i = origin.x; i < row; i++) { 
                Vector2Int newCoor = new Vector2Int(i, origin.y);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
                if (!isPiercing && i != origin.x && IsThereAnEntity(newCoor)) { break; } // Stop moving right if not piercing and entity is found
            }

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight horizontal line on the same row as origin
        public static List<Vector2Int> ReturnPosList_Horizontal(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            // Combines left and right pos lists while also checking for piercing in both
            List<Vector2Int> validPosList = ReturnPosList_Left(origin, isPiercing, includeOrigin).Concat(ReturnPosList_Right(origin, isPiercing, includeOrigin)).ToList();

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight line above origin
        public static List<Vector2Int> ReturnPosList_Up(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            for (int i = origin.y; i > 0; i--) { 
                Vector2Int newCoor = new Vector2Int(origin.x, i);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
                if (!isPiercing && i != origin.y && IsThereAnEntity(newCoor)) { break; } // Stop moving up if not piercing and entity is found
            }

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight line below origin
        public static List<Vector2Int> ReturnPosList_Down(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            for (int i = origin.y; i < col; i++) { 
                Vector2Int newCoor = new Vector2Int(origin.x, i);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
                if (!isPiercing && i != origin.y && IsThereAnEntity(newCoor)) { break; } // Stop moving down if not piercing and entity is found
            }

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a straight line below origin
        public static List<Vector2Int> ReturnPosList_Vertical(Vector2Int origin, bool isPiercing = true, bool includeOrigin = false) {
            // Combines up and down pos lists while also checking for piercing in both
            List<Vector2Int> validPosList = ReturnPosList_Up(origin, isPiercing, includeOrigin).Concat(ReturnPosList_Down(origin, isPiercing, includeOrigin)).ToList();

            return ReturnFinishedPosList(origin, validPosList, includeOrigin);
        }

        // Returns a pos list without any duplicates and possibly include origin depending on includeOrigin bool
        private static List<Vector2Int> ReturnFinishedPosList(Vector2Int origin, List<Vector2Int> posList, bool includeOrigin) { 
            if (includeOrigin) { posList.Add(origin); }
            else { posList.RemoveAll((v2) => v2 == origin); } // Remove any points that are equal to origin in case someone added it accidentally

            return ReturnDistinctPosList(posList);
        }

        // Returns entities that are not the attacker's faction
        private static List<Entity> GetEntitiesOnMap(Faction attackerFaction) {
            List<Faction> targetFactions = System.Enum.GetValues(typeof(Faction)).Cast<Faction>().ToList(); // Remove attacker faction from faction list
            targetFactions.Remove(attackerFaction);

            return entitiesPos.Keys.ToList().Where((entity) => targetFactions.Contains(entity.GetFaction())).ToList(); // Return entities that belong to the remain factions
        }
    }

    // Grid array must be initialized before Start()
    void Awake() {
        // Instance declaration
        if (Inst != null && Inst != this) { Destroy(this); }
        else { Inst = this; }
        
        // Sanity checks
        if (!turnsHandler) turnsHandler = this.GetComponent<World_Turns>();

        // Store/Init all grid nodes into "grid" 2d array
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        int childCounter = 0;

        for(int x = 0; x < GetRow(); x++) {
            for(int y = 0; y < GetCol(); y++) {
                // Debug.Log("Node [" + x + ", " + y + "]: " + childCounter);
                
                World_GridNode currNode = RecordNode(gridNodeParent.GetChild(childCounter).GetComponent<World_GridNode>(), x, y);

                currNode.gameObject.name = "Node [" + x + ", " + y + "]"; // Set node name for easier debugging
                currNode.SetIsPlayerControlled(x < 3); // Set right half of grid to enemy control

                childCounter++;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}