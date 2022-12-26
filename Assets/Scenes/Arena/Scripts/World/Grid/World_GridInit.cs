using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// This script contains the World_Grid static class
// The MonoBehaviour class is simply to initialize some things required by World_Grid
public class World_GridInit : MonoBehaviour {
    [SerializeField] private World_Turns turnsHandler;
    [SerializeField] private Transform gridNodeParent;

    // Grid array must be initialized before Start()
    void Awake() {
        int childCounter = 0;

        // Store/Init all grid nodes into "grid" 2d array
        for(int x = 0; x < World_Grid.GetRow(); x++) {
            for(int y = 0; y < World_Grid.GetCol(); y++) {
                // Debug.Log("Node [" + x + ", " + y + "]: " + childCounter);
                
                World_GridNode currNode = World_Grid.RecordNode(gridNodeParent.GetChild(childCounter).GetComponent<World_GridNode>(), x, y);

                currNode.gameObject.name = "Node [" + x + ", " + y + "]"; // Set node name for easier debugging
                currNode.SetIsPlayerControlled(x < 3); // Set right half of grid to enemy control

                childCounter++;
            }
        }

        World_Grid.SetTurnsHandler(turnsHandler);
    }
}

// Stores information about the grid
// Handles movement and combat which is tied to the grid
public static class World_Grid {
    private static World_Turns turnsHandler;

    private static int row = 6, col = 3;
    private static World_GridNode[,] grid = new World_GridNode[row, col]; // A 2D array that goes from left to right, top to bottom;
    private static Dictionary<IEntity, Vector2Int> entitiesPos = new Dictionary<IEntity, Vector2Int>(); // Contains all entities on grid and their position

    // Getters
    public static int GetRow() { return row; }
    public static int GetCol() { return col; }
    public static World_GridNode GetNode(int x, int y) { return isCoorInGrid(x, y) ? grid[x, y] : null; }
    public static World_GridNode GetNode(Vector2Int vec2) { return isCoorInGrid(vec2) ? grid[vec2.x, vec2.y] : null; }
    public static Vector2Int GetEntityGridPos(IEntity entity) { return entitiesPos.ContainsKey(entity) ? entitiesPos[entity] : new Vector2Int(); }

    // Setters
    public static void SetTurnsHandler(World_Turns handler) { turnsHandler = handler; }
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
        public static void SetGridPos(IEntity entity, Vector2Int vec2) {
            // Debug.Log("SetGridPos: " + entity + ", " + vec2);

            World_GridNode targetNode = GetNode(vec2);

            // Sanity check in case node is out of bounds or does not have a World_GridNode component attached
            if (targetNode) {
                if (!entitiesPos.ContainsKey(entity)) { entitiesPos.Add(entity, vec2); } // Register new entitiesPos key
                else { entitiesPos[entity] = vec2; } // Update entitiesPos

                // Move entity to new grid position
                // Use world position for accuracy
                entity.GameObj.transform.position = targetNode.gameObject.transform.position;
            }
            else {
                Debug.LogWarning("Node is out of bounds or does not have component attached!");
            }
        }

        // SetGridPos() with a "entities cannot move to a grid node with another entity on it" restriction
        public static void MoveToPos(IEntity entity, Vector2Int gridCoor) {
            if (!entitiesPos.Values.ToList().Contains(gridCoor)) { SetGridPos(entity, gridCoor); }
        }

        // Some overloaded functions in case you just want to pass 2 ints instead
        public static void SetGridPos(IEntity entity, int x, int y) { SetGridPos(entity, new Vector2Int(x, y)); }
        public static void MoveToPos(IEntity entity, int x, int y) { MoveToPos(entity, new Vector2Int(x, y)); }
    }

    public static class Combat {
        // Overloaded functions to check if entity is on specfic grid pos
        public static bool IsEntityHere(IEntity entity, Vector2Int pos) { return entitiesPos[entity] == pos; }
        public static bool IsEntityHere(IEntity entity, List<Vector2Int> posList) { return posList.Contains(entitiesPos[entity]); }

        // Returns a valid pos list that is relative to the origin point
        // relativePosList MUST be values that are relative to origin point
        // eg. origin: (0, 0), relativePosList: [(-1, 0)] means the post list will be the left of origin
        // Set includeOrigin to false if you don't want to include the origin point
        public static List<Vector2Int> ReturnRelativePosList(Vector2Int origin, List<Vector2Int> relativePosList, bool includeOrigin = true) {
            List<Vector2Int> validPosList = new List<Vector2Int>();

            // Calculate and add any pos that are within the grid
            foreach (Vector2Int v2 in relativePosList) {
                int newX = v2[0] + origin[0];
                int newY = v2[1] + origin[1];

                Vector2Int newCoor = new Vector2Int(newX, newY);

                if (isCoorInGrid(newCoor)) { validPosList.Add(newCoor); }
            }

            if (includeOrigin) { validPosList.Add(origin); }
            else { validPosList.RemoveAll((v2) => v2 == origin); } // Remove any points that are equal to origin in case someone added it accidentally

            return ReturnDistinctPosList(validPosList); // Remove any duplicates 
        }

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

        public static void HitHere(Faction attackerFaction, List<Vector2Int> posList, int dmg) {
            ReturnDistinctPosList(posList); // Remove any duplicates 

            foreach (IEntity entity in GetEntitiesOnMap(attackerFaction)) {
                if (IsEntityHere(entity, posList)) { ResolveHitsOnEntity(entity, dmg); }
            }
        }

        // Returns a pos list without any duplicates
        private static List<Vector2Int> ReturnDistinctPosList(List<Vector2Int> posList) { return posList.Distinct().ToList(); }

        // Removes an entity from the library
        // Usually used when an enemy dies
        private static void RemoveEntityFromGrid(IEntity entity) { 
            entitiesPos.Remove(entity); 
            turnsHandler.RemoveAllTurnsByEntity(entity);
        }

        // Returns entities that are not the attacker's faction
        private static List<IEntity> GetEntitiesOnMap(Faction attackerFaction) {
            List<Faction> targetFactions = System.Enum.GetValues(typeof(Faction)).Cast<Faction>().ToList(); // Remove attacker faction from faction list
            targetFactions.Remove(attackerFaction);

            return entitiesPos.Keys.ToList().Where((entity) => targetFactions.Contains(entity.Faction)).ToList(); // Return entities that belong to the remain factions
        }

        private static void ResolveHitsOnEntity(IEntity entity, int damage) {
            entity.OnHit(damage); 

            // Entity dies here
            if (entity.Health <= 0) { 
                entity.OnDeath(); 
                RemoveEntityFromGrid(entity);
            }
        }
    }
}