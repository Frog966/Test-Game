using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    // Used as seed for Unity randomizer
    //! Unity's randomizer seed cannot be retrieved after it is set so it's best to generate our own and set it so that we have a record of it
    [SerializeField] private int randSeed;

    public Player player;
    public Transform enemyParent;

    // Getters
    public int GetSeed_Int() { return randSeed; }

    void Awake() {
        randSeed = UnityEngine.Random.Range(0, System.Int32.MaxValue); // Randomize randomizer seed
        Random.InitState(randSeed); // Set randomizer seed
        // Random.InitState(123456); // Test randomizer seed 
        
        // Destroy every GO in world.enemyParent
        // enemyParent will be populated in the future
        foreach (Transform child in enemyParent) {
            Destroy(child.gameObject);
        }
    }

    void Start() {
        Debug.Log("randSeed: " + randSeed);
    }
}