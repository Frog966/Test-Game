using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {
    private Player player;

    [SerializeField] private World_Grid grid;

    public void ResetCost() { player.moveCost = player.moveCostTrue; }

    // World_Grid.SetGridPos() but with restrictions
    public void MoveTo(Vector2Int vec2) {
        World_GridNode node = grid.GetNode(vec2);

        // Player must have enough energy to move and can only move onto player-controlled nodes 
        if (player.energy - player.moveCost >= 0 && node && node.isPlayerControlled) {
            grid.MoveToPos(player.gameObject, vec2);
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

    // Start is called before the first frame update
    void Start() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();

        ResetCost();
    }
}