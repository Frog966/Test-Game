using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Map;

public class World_MapNode : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private float lineWidth = 5; // It's actually line height

    [Header("Important Stuff")]
    [SerializeField] private Transform lineParent;
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private UnityEngine.UI.Button buttonScript;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite icon_Fight;
    [SerializeField] private Sprite icon_MiniBoss;
    [SerializeField] private Sprite icon_Boss;
    [SerializeField] private Sprite icon_Shop;

    [SerializeField] private List<EncounterEnemyDetails> encounter = new List<EncounterEnemyDetails>(); // A dictionary containing enemies and where they're located on the grid
    
    private NodeType nodeType;
    private List<World_MapNode> nextNodes = new List<World_MapNode>(); // A list containing map nodes that this node can move to
    private List<World_MapNode> prevNodes = new List<World_MapNode>(); // A list containing map nodes that is connected to this node. Is simply a record to make sure all map nodes will lead to the end

    // Getters
    public NodeType GetNodeType() { return nodeType; }
    public List<EncounterEnemyDetails> GetEncounter() { return encounter; }
    public List<World_MapNode> GetPrevNodes() { return prevNodes; }
    public List<World_MapNode> GetNextNodes() { return prevNodes; }

    // Enable this node to be moved onto
    public void EnableNode() {
        buttonScript.enabled = true;
        icon.color = Color.white; 
    }

    // Disable this node from being moved onto
    public void DisableNode() {
        buttonScript.enabled = false;
        icon.color = Color.gray; 
    }

    // Constructor
    public void Setup(NodeType _nodeType, List<World_MapNode> _nextNodes, List<EncounterEnemyDetails> _encounter, Action<World_MapNode> _onClick) {
        Reset();

        nodeType = _nodeType;
        nextNodes = _nextNodes;
        encounter = _encounter;
        buttonScript.onClick.AddListener(() => _onClick(this));
        
        // Instantiate new lines if not enough for each next node
        while (lineParent.childCount < nextNodes.Count) {
            Transform newLine = GameObject.Instantiate(lineParent.GetChild(0), lineParent).transform; // Clone a line
            
            newLine.localPosition = Vector3.zero;
            newLine.localRotation = Quaternion.Euler(0, 0, 0);
            newLine.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth, lineWidth);
        }

        // Set icons based on nodeType
        switch (nodeType) {
            case NodeType.ENEMY:
                icon.sprite = icon_Fight;
                break;
            case NodeType.MINI_BOSS:
                icon.sprite = icon_MiniBoss;
                break;
            case NodeType.BOSS:
                icon.sprite = icon_Boss;
                break;
            case NodeType.SHOP:
                icon.sprite = icon_Shop;
                break;
            default:
                icon.sprite = null;
                break;
        }

        // Setup the lines between nodes
        // Basically the current node will extend their lines to their valid nodes at the correct angle
        for (int i = 0; i < nextNodes.Count; i++) {
            Transform currNode = this.transform;
            Transform nextNode = nextNodes[i].transform;
            Transform currLine = lineParent.GetChild(i);

            float x1 = currNode.localPosition.x;
            float x2 = nextNode.localPosition.x;
            float y1 = currNode.localPosition.y;
            float y2 = nextNode.localPosition.y;

            // Debug.Log(currNode.gameObject.name + ": " + currNode.localPosition + ", " + nextNode.gameObject.name + ": " + nextNode.localPosition);

            // Set line angle
            currLine.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(y2 - y1, x2 - x1) * (180.0f / Mathf.PI));

            // Set line width using Pythagorean theorem
            RectTransform currRectTrans = currLine.GetComponent<RectTransform>();
            currRectTrans.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2)), currRectTrans.rect.height);
        }
    }

    public void Reset() {
        encounter.Clear();
        nextNodes.Clear();
        prevNodes.Clear();

        // Reset lines
        foreach (Transform line in lineParent) {
            line.localRotation = Quaternion.Euler(0, 0, 0);
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth, lineWidth);
        }
    }

    public void AddPrevNode(World_MapNode prevNode) {
        // Only add to prevNodes list if prevNode is not in the list to avoid duplicates
        if (!prevNodes.Contains(prevNode)) { prevNodes.Add(prevNode); }
    }
    
    void Awake() {
        // Sanity checks
        if (buttonScript == null) {
            buttonScript = this.gameObject.GetComponent<UnityEngine.UI.Button>() as UnityEngine.UI.Button;
            
            if (buttonScript == null) {
                buttonScript = this.gameObject.AddComponent<UnityEngine.UI.Button>() as UnityEngine.UI.Button;
            }
        }
    }
}