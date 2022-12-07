using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    // Used as seed for Unity randomizer
    //! Unity's randomizer seed cannot be retrieved after it is set so it's best to generate our own and set it so that we have a record of it
    [SerializeField] private int randSeed;

    [SerializeField] private Player player;
    [SerializeField] private Transform enemyParent;

    // Handlers
    private World_Map _mapHandler;
    private World_Grid _gridHandler;

    // Getters
    public int GetSeed() { return randSeed; }
    public Player Player() { return player; }
    public Transform EnemyParent() { return enemyParent; }

    // Handler Getters
    public World_Map MapHandler() { return _mapHandler; }
    public World_Grid GridHandler() { return _gridHandler; }

    void Awake() {
        // Sanity checks
        if (_mapHandler == null) _mapHandler = this.gameObject.GetComponent<World_Map>();
        if (_gridHandler == null) _gridHandler = this.gameObject.GetComponent<World_Grid>();

        randSeed = UnityEngine.Random.Range(0, System.Int32.MaxValue); // Randomize randomizer seed
        Random.InitState(randSeed); // Set randomizer seed
        // Random.InitState(123456); // Test randomizer seed 
        
        // Destroy every GO in world.enemyParent
        // enemyParent will be populated in the future
        foreach (Transform child in enemyParent) { Destroy(child.gameObject); }
    }

    void Start() {
        Debug.Log("randSeed: " + randSeed);
    }
}