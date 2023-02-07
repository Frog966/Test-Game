using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestMiniboss : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    private string id = "Test Miniboss";
    private Queue<Turn> turnQueue = new Queue<Turn>();

    // Properties
    public string ID { get => id; }
    public Entity Entity { get => entity; }
    public Queue<Turn> TurnQueue { get => turnQueue; }

    // The enemy's AI
    public Turn ReturnCurrentTurn() {
        if (turnQueue.Count > 0) { return turnQueue.Dequeue(); }
        else {
            Queue<IEnumerator> newTurn = new Queue<IEnumerator>();

            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                case 0: {
                    return new Turn(new Queue<IEnumerator>());
                }
                default: {
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds!");

                    return null;
                }
            }
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!entity) entity = this.GetComponent<Entity>();
    }
}