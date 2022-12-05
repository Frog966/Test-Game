using UnityEngine;

public class World_Grid : MonoBehaviour {
    public static int row = 6, col = 3;

    // A 2D array that goes from left to right, top to bottom;
    private World_GridNode[,] grid = new World_GridNode[row, col]; 

    public Transform nodeParent;

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

    private bool isNodeInBounds(Vector2Int vec2) {
        if (vec2.x < 0 || vec2.x >= row || vec2.y < 0 || vec2.y >= col) { return false; }

        return true;
    }

    // Getters
    public World_GridNode GetNode(Vector2Int vec2) { return isNodeInBounds(vec2) ? grid[vec2.x, vec2.y] : null; }
}