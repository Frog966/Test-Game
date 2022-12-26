using UnityEngine;

public class Player_KeyPresses : MonoBehaviour {
    [SerializeField] private Player_Movement movementHandler;

    void Start() {
        //! Sanity Checks
        if (!movementHandler) movementHandler = this.gameObject.GetComponent<Player_Movement>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow)) { movementHandler.MoveUp(); }
        if (Input.GetKeyDown("a") || Input.GetKeyDown(KeyCode.LeftArrow)) { movementHandler.MoveLeft(); }
        if (Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow)) { movementHandler.MoveDown(); }
        if (Input.GetKeyDown("d") || Input.GetKeyDown(KeyCode.RightArrow)) { movementHandler.MoveRight(); }
    }
}