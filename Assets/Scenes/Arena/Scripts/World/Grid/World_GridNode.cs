// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class World_GridNode : MonoBehaviour {
    [SerializeField] private bool isPlayerControlled = true;
    
    [SerializeField] private UnityEngine.UI.Image top, front;
    [SerializeField] private Color color_Player, color_Enemy;

    // Getters
    public bool IsPlayerControlled() { return isPlayerControlled; }

    // Set isPlayerControlled + update image color accordingly
    public void SetIsPlayerControlled(bool b) {
        // Debug.Log("setIsPlayerControlled: " + b);

        isPlayerControlled = b;
        top.color = front.color = isPlayerControlled ? color_Player : color_Enemy; // Set image according to isPlayerControlled bool
    }
}