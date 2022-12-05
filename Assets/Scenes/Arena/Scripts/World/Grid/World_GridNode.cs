// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class World_GridNode : MonoBehaviour {
    public bool isPlayerControlled = true;
    
    public UnityEngine.UI.Image image;
    public Sprite gridPlayer;
    public Sprite gridEnemy;

    // Start is called before the first frame update
    void Start() {
        // Sanity Check
        if (image == null) this.gameObject.GetComponent<UnityEngine.UI.Image>();
    }

    // Set isPlayerControlled bool + update sprite accordingly
    public void SetIsPlayerControlled(bool b) {
        // Debug.Log("setIsPlayerControlled: " + b);

        isPlayerControlled = b;
        image.sprite = isPlayerControlled ? gridPlayer : gridEnemy; // Set image according to isPlayerControlled bool
    }
}