using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Map;

// In charge of storing/display the map as well as instantiate encounters
public class World_Map : MonoBehaviour {
    private World world;
    private World_EnemyLibrary enemyLibrary;

    // A 2D array that represents an array of "layers" each containing an array of map nodes the player can traverse
    // Each player can only go to MapNodes on the next layer from where they are currently on but only certain nodes are valid
    private List<List<World_MapNode>> map;

    public MapLocale currLocale;
    public Transform nodePool, nodeParent;
    public GameObject mapNodePrefab; // Sprite holder and clickable object

    // Getters
    public World_MapNode GetMapNode(int x, int y) { return map[x][y]; }

    // Generates a tree that will be used as the map
    private void GenerateMap() {
        // Debug.Log("Generating map");

        // Randomizing num. of layers
        map = new List<List<World_MapNode>>(new List<World_MapNode>[UnityEngine.Random.Range(5, 5)]);

        // Debug.Log("Layer Count: " + map.Count);

        // Generate map
        // Going through layers from end to beginning so we can generate valid nodes in 1 go
        for (int x = map.Count - 1; x >= 0; x--) {
            // Debug.Log("Layer " + x + " - Map Node Count: " + map[x].Count);

            // Randomizing num. of nodes in current layer except final node
            map[x] = new List<World_MapNode>(new World_MapNode[x != map.Count - 1 ? UnityEngine.Random.Range(3, 6) : 1]);

            for (int y = 0; y < map[x].Count; y++) {
                GameObject currMapNodeObj;

                // Recycle any already created map nodes else instantiate a prefab to nodeParent
                if (nodePool.childCount > 0) {
                    currMapNodeObj = nodePool.GetChild(0).gameObject;
                    currMapNodeObj.transform.SetParent(nodeParent);
                }
                else {
                    currMapNodeObj = Instantiate(mapNodePrefab, nodeParent);
                }
                
                // Rename map nodes according to their position in array
                currMapNodeObj.name = "[" + x.ToString() + ", " + y.ToString() + "]";

                // Setup how the map node objs are displayed
                //-----------------------------------------------------------------------------------------------------------------------
                float xOffset = nodeParent.GetComponent<RectTransform>().rect.width / (float)(map.Count - 1);
                float yOffset = 75.0f;

                // Set currMapNodeObj local pos
                //! This is all related to nodeParent's pivot which is (0, 0.5). Do not change the pivot
                currMapNodeObj.transform.localPosition = new Vector2(
                    -(nodeParent.GetComponent<RectTransform>().rect.width / 2.0f) + (xOffset * x),
                    (yOffset * (float)(map[x].Count - 1) / 2.0f) - (yOffset * (float)(y))
                );
                //-----------------------------------------------------------------------------------------------------------------------

                // Registering a World_MapNode script as an element in map as well as setting up the script
                //-----------------------------------------------------------------------------------------------------------------------
                List<World_MapNode> newNextNodes = GenerateNextNodesList(x, y);
                NodeType newNodeType = RandomizeNodeType(x, newNextNodes);

                // Store World_MapNode class into map
                // Do this last because we need name and local pos set up first
                map[x][y] = currMapNodeObj.GetComponent<World_MapNode>();
                map[x][y].Setup(
                    newNodeType,
                    newNextNodes,
                    enemyLibrary.ReturnRandomEncounter(currLocale, newNodeType)
                );
                //-----------------------------------------------------------------------------------------------------------------------
            }
        }

        // Returns a random node type
        NodeType RandomizeNodeType(int currX, List<World_MapNode> nextNodes) {
            if (currX >= map.Count - 1) { // Final node is always boss
                return NodeType.BOSS;
            }
            else if (currX <= 0) { // Final node is always enemy
                return NodeType.ENEMY;
            }
            else {
                List<NodeType> EnumValues = Enum.GetValues(typeof(NodeType)).Cast<NodeType>().ToList();

                // Debug.Log("Test: " + EnumValues.Count);
                
                // Prevent certain nodes from appearing consecutively
                foreach (World_MapNode node in nextNodes) {
                    NodeType nodeType = node.GetNodeType();

                    if ((nodeType == NodeType.SHOP || nodeType == NodeType.MINI_BOSS) && EnumValues.Contains(nodeType)) {
                        EnumValues.Remove(nodeType);
                    }
                }

                // BOSS should be excluded from randomizer so start at 1
                return EnumValues[UnityEngine.Random.Range(1, EnumValues.Count)];
            }
        }

        // Generates a list of valid nodes for the current node represented by currX and currY
        // It is not true random as there are limitations on where current node can go. eg. it can only go to a node that is 1 index away from itself (-1, 0 , 1)
        List<World_MapNode> GenerateNextNodesList(int currX, int currY) {
            // Debug.Log("GenerateNextNodesList [" + currX + ", " + currY + "]");
            
            // No need to set node nexts if at final layer
            if (currX >= map.Count - 1) {
                return Enumerable.Empty<World_MapNode>().ToList(); // Return an immutable empty list
            }
            else {
                // It's always a node on the next layer
                int nextX = currX + 1;
                List<World_MapNode> validNodes = new List<World_MapNode>(); // Stores all valid nodes for randomization later

                // Determine which nodes on the next layer can be reached by the current node
                //-----------------------------------------------------------------------------------------------------------------------
                if (currY <= map[nextX].Count - 1) { validNodes.Add(map[nextX][currY]); } // Middle element (y) is valid
                if (currY + 1 <= map[nextX].Count - 1) { validNodes.Add(map[nextX][currY + 1]); } // Lower element (y + 1) is valid
                if (currY - 1 >= 0 && currY - 1 <= map[nextX].Count - 1) { validNodes.Add(map[nextX][currY - 1]); } // Upper element (y - 1) is valid
                //-----------------------------------------------------------------------------------------------------------------------

                // If more than 1 valid node, remove a single possible route or none at all
                // Nothing happens if only 1 valid node
                if (validNodes.Count > 1) {
                    int rand = UnityEngine.Random.Range(0, validNodes.Count);
                    
                    if (rand != validNodes.Count) { validNodes.RemoveAt(rand); }

                    // Check if next upper node has no prev nodes as this is the last chance to attach current node to it
                    // Also avoiding duplicates
                    if (currY - 1 >= 0 && map[nextX][currY - 1].GetPrevNodes().Count < 1 && !validNodes.Contains(map[nextX][currY - 1])) {
                        validNodes.Add(map[nextX][currY - 1]);
                    }
                }
                else if (validNodes.Count < 1) {
                    // Sanity checking if current node has no valid nodes which means current node's index is greater than next layer's final node's index by at least 2
                    // Add next layer's final node to validNodes
                    validNodes.Add(map[nextX][map[nextX].Count - 1]);
                }

                // If is last map node of current layer, double check and add any nodes on the next layer without any prev nodes
                if (currY == map[currX].Count - 1) {
                    // Start from middle layer since we already will add upper node
                    for (int i = currY; i < map[nextX].Count; i++) {
                        World_MapNode nextNode = map[nextX][i];

                        if (!validNodes.Contains(nextNode)) { validNodes.Add(nextNode); }
                    }
                }
                
                // Register current node as previous node for valid nodes
                foreach (World_MapNode node in validNodes) { node.AddPrevNode(map[currX][currY]); }

                return validNodes;
            }
        }
    }

    void Awake() {
        // Sanity checks
        world = this.gameObject.GetComponent<World>();
        if (enemyLibrary == null) enemyLibrary = this.gameObject.GetComponent<World_EnemyLibrary>();

        nodePool.gameObject.SetActive(false);
        nodeParent.gameObject.SetActive(true);

        // Destroy any GOs that do not have Image component in nodePool
        foreach (Transform child in nodePool) {
            if (!child.GetComponent<Image>()) {
                child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
                Destroy(child.gameObject);
            }
        }

        // Destroy any GOs that do not have Image component in nodeParent
        // Move any nodes in nodeParent to nodePool
        foreach (Transform child in nodeParent) {
            if (child.GetComponent<Image>()) {
                child.SetParent(nodePool);
            }
            else {
                child.SetParent(child.parent.parent); // Change parent first to avoid any complications as Destroy() only happens at end of frame
                Destroy(child.gameObject);
            }
        }
    }

    void Start() {
        GenerateMap();
    }
}