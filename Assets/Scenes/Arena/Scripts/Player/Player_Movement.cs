using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {
    private Player player;

    [SerializeField] private World_Grid gridHandler;

    public void ResetMoveCost() { player.moveCost = player.moveCostTrue; }

    // World_Grid.SetGridPos() but with restrictions
    public void MoveTo(Vector2Int vec2) {
        World_GridNode node = gridHandler.GetNode(vec2);

        // Player must have enough energy to move and can only move onto player-controlled nodes 
        if (player.energy - player.moveCost >= 0 && node && node.IsPlayerControlled()) {
            gridHandler.MoveToPos(player, vec2);
            player.EnergyHandler().DecreaseEnergy(player.moveCost); // Each move lowers energy
        }
        // else {
        //     if (player.energy - player.moveCost < 1) {
        //         Debug.Log("Player has not enough energy! Move Cost: " + player.moveCost + ". Current Energy: " + player.energy);
        //     }
        //     else if (!node) {
        //         Debug.Log("Player is moving to an invalid node! Target Node: " + x + ", " + y);
        //     }
        //     else if (!node.IsPlayerControlled) {
        //         Debug.Log("Player is moving to an enemy-controlled node! Target Node: " + x + ", " + y);
        //     }
        // }
    }

    // Shortcuts
    //! Up and down is reversed because of how the grid is setup
    public void MoveUp() { MoveTo(gridHandler.GetEntityGridPos(player) + Vector2Int.down); }
    public void MoveLeft() { MoveTo(gridHandler.GetEntityGridPos(player) + Vector2Int.left); }
    public void MoveDown() { MoveTo(gridHandler.GetEntityGridPos(player) + Vector2Int.up); }
    public void MoveRight() { MoveTo(gridHandler.GetEntityGridPos(player) + Vector2Int.right); }

    void Awake() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();

        // Non-player handlers sanity checks
        if (gridHandler == null) { gridHandler = this.transform.parent.GetComponentInChildren(typeof(World_Grid)) as World_Grid; }
    }

    void Start() {
        ResetMoveCost();
    }
}