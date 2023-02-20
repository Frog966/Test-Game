using UnityEngine;

public class Player_Movement : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private Player_Energy energyHandler;
    [SerializeField] private int moveCostTrue = 1, moveCost; // Movement cost

    public void ResetMoveCost() { moveCost = moveCostTrue; }

    // World_Grid.Movement.SetGridPos() but with restrictions
    public void MoveTo(Vector2Int vec2) {
        if (!AnimHandler.isAnimating) {
            World_GridNode node = World_Grid.GetNode(vec2);

            // Player must have enough energy to move and can only move onto player-controlled nodes 
            if (energyHandler.CanPayEnergyCost(moveCost) && node && node.IsPlayerControlled() && !World_Grid.IsThereAnEntity(vec2)) {
                World_Grid.Movement.MoveToPos(Player.GetEntity(), vec2);
                energyHandler.DecreaseEnergy(moveCost); // Each move lowers energy

                Player.GetEntity().PlayAnimation("Move");
            }
            else {
                if (!energyHandler.CanPayEnergyCost(moveCost)) { energyHandler.NotEnoughEnergy(); }
                // else if (!node) {
                //     Debug.Log("Player is moving to an invalid node! Target Node: " + x + ", " + y);
                // }
                // else if (!node.IsPlayerControlled) {
                //     Debug.Log("Player is moving to an enemy-controlled node! Target Node: " + x + ", " + y);
                // }
            }
        }
    }

    // Shortcuts
    //! Up and down is reversed because of how the grid is setup
    public void MoveUp() { MoveTo(World_Grid.GetEntityGridPos(Player.GetEntity()) + Vector2Int.down); }
    public void MoveLeft() { MoveTo(World_Grid.GetEntityGridPos(Player.GetEntity()) + Vector2Int.left); }
    public void MoveDown() { MoveTo(World_Grid.GetEntityGridPos(Player.GetEntity()) + Vector2Int.up); }
    public void MoveRight() { MoveTo(World_Grid.GetEntityGridPos(Player.GetEntity()) + Vector2Int.right); }

    void Awake() {
        //! Sanity Checks
        if (!player) player = this.gameObject.GetComponent<Player>();
        if (!energyHandler) energyHandler = this.gameObject.GetComponent<Player_Energy>();
    }

    void Start() {
        ResetMoveCost();
    }
}