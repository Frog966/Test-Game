using UnityEngine;

public class Player_KeyPresses : MonoBehaviour {
    private Player player;

    void Start() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow)) {
            // print("up");

            player.MovementHandler().MoveUp();
        }

        if (Input.GetKeyDown("a") || Input.GetKeyDown(KeyCode.LeftArrow)) {
            // print("left");

            player.MovementHandler().MoveLeft();
        }

        if (Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow)) {
            // print("down");

            player.MovementHandler().MoveDown();
        }
        
        if (Input.GetKeyDown("d") || Input.GetKeyDown(KeyCode.RightArrow)) {
            // print("right");

            player.MovementHandler().MoveRight();
        }
    }
}