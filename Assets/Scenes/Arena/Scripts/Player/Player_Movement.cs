using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {
    private Player player;
    private Transform playerTrans; // A reference to player transform since we'll be primarily using this

    public World_Grid grid;

    // Start is called before the first frame update
    void Start() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();
        
        playerTrans = player.playerObj.gameObject.transform;

        ResetCost();
        SetGridPos(Vector2Int.one);
    }

    public void ResetCost() {
        player.moveCost = player.moveCostTrue;
    }

    // Set player grid position
    public void SetGridPos(Vector2Int vec2) {
        World_GridNode node = grid.GetNode(vec2);

        // Sanity check in case node is out of bounds or does not have a World_GridNode component attached
        if (node) {
            // Move player object to new position
            Vector3 newPos = node.gameObject.transform.position;
            playerTrans.position = newPos;

            // Update playerCoor
            player.playerCoor = vec2;

            // Debug.Log("New Player Pos: " + newPos);
        }
        // else {
        //     Debug.Log("Node is out of bounds or does not have component attached!");
        // }
    }

    // Similar to SetGridPos() but has restrictions
    public void MoveTo(Vector2Int vec2) {
        World_GridNode node = grid.GetNode(vec2);

        // Player must have enough energy to move and can only move onto player-controlled nodes 
        if (player.energy - player.moveCost >= 0 && node && node.isPlayerControlled) {
            SetGridPos(vec2);
            player.EnergyHandler().DecreaseEnergy(player.moveCost); // Each move lowers energy
        }
        // else {
        //     if (player.energy - player.moveCost < 1) {
        //         Debug.Log("Player has not enough energy! Move Cost: " + player.moveCost + ". Current Energy: " + player.energy);
        //     }
        //     else if (!node) {
        //         Debug.Log("Player is moving to an invalid node! Target Node: " + x + ", " + y);
        //     }
        //     else if (!node.isPlayerControlled) {
        //         Debug.Log("Player is moving to an enemy-controlled node! Target Node: " + x + ", " + y);
        //     }
        // }
    }

    // Shortcuts
    //! Up and down is reversed because of how we setup the grid
    public void MoveUp() { MoveTo(player.playerCoor + Vector2Int.down); }
    public void MoveLeft() { MoveTo(player.playerCoor + Vector2Int.left); }
    public void MoveDown() { MoveTo(player.playerCoor + Vector2Int.up); }
    public void MoveRight() { MoveTo(player.playerCoor + Vector2Int.right); }
}