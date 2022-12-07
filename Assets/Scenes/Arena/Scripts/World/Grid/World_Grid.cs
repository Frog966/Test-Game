using System.Collections.Generic;
using UnityEngine;

public class World_Grid : MonoBehaviour {
    [SerializeField] private static int row = 6, col = 3;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Dictionary<GameObject, Vector2Int> entitiesOnGrid = new Dictionary<GameObject, Vector2Int>();

    // A 2D array that goes from left to right, top to bottom;
    private World_GridNode[,] grid = new World_GridNode[row, col]; 

    // Getters
    public World_GridNode GetNode(Vector2Int vec2) { return isNodeInBounds(vec2) ? grid[vec2.x, vec2.y] : null; }
    public Vector2Int GetEntityGridPos(GameObject entity) { return entitiesOnGrid.ContainsKey(entity) ? entitiesOnGrid[entity] : new Vector2Int(); }

    // Set entities' grid position
    // Registers/updates the entitiesOnGrid when an entity moves to a grid position
    // Has no restrictions other than requiring a valid grid pos
    public void SetGridPos(GameObject entity, Vector2Int vec2) {
        // Debug.Log("SetGridPos: " + entity + ", " + vec2);

        World_GridNode node = GetNode(vec2);

        // Sanity check in case node is out of bounds or does not have a World_GridNode component attached
        if (node) {
            if (!entitiesOnGrid.ContainsKey(entity)) { entitiesOnGrid.Add(entity, vec2); } // Register new entitiesOnGrid key
            else { entitiesOnGrid[entity] = vec2; } // Update entitiesOnGrid

            // Move entity to new grid position
            // Use world position for accuracy
            entity.transform.position = node.gameObject.transform.position;
        }
        // else {
        //     Debug.Log("Node is out of bounds or does not have component attached!");
        // }
    }

    // Removes an entity from the library
    // Usually when an enemy dies
    public void RemoveEntityFromGrid(GameObject entity) { entitiesOnGrid.Remove(entity); }

    private bool isNodeInBounds(Vector2Int vec2) {
        if (vec2.x < 0 || vec2.x >= row || vec2.y < 0 || vec2.y >= col) { return false; }

        return true;
    }

    // Grid array must be initialized before Start()
    void Awake() {
        int counter = 0;

        // Store/Init all grid nodes into array
        for(int x = 0; x < row; x++) {
            for(int y = 0; y < col; y++) {
                // Debug.Log("Node [" + x + ", " + y + "]: " + counter);
                
                grid[x, y] = nodeParent.GetChild(counter).GetComponent<World_GridNode>();
                grid[x, y].gameObject.name = "Node [" + x + ", " + y + "]"; // Set node name for easier debugging
                grid[x, y].SetIsPlayerControlled(x < 3); // Set right half of grid to enemy control

                counter++;
            }
        }
    }
}