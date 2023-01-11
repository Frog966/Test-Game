using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestMiniboss : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    private string id = "Test Miniboss";

    // Properties
    public string ID { get => id; }
    public Entity Entity { get => entity; }

    // This AI has patterns that span multiple turns
    private Queue<Queue<IEnumerator>> turnQueue = new Queue<Queue<IEnumerator>>();

    // The enemy's AI
    public Queue<IEnumerator> ReturnCurrentTurn() {
        if (turnQueue.Count > 0) { return turnQueue.Dequeue(); }
        else {
            Queue<IEnumerator> newTurn = new Queue<IEnumerator>();

            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                case 0: {
                    List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(
                        // new Vector2Int(1, 1),
                        new Vector2Int(0, 0),
                        new List<Vector2Int>() { 
                            new Vector2Int(-1, 0),
                            new Vector2Int(1, 0),
                            new Vector2Int(0, -1),
                            new Vector2Int(0, 1)
                        }
                    );

                    // AddToTurnQueue(new IEnumerator[] { World_Grid.TelegraphHere(posList) });
                    AddToTurnQueue(new IEnumerator[] { Attack() });
                    
                    return turnQueue.Dequeue();

                    IEnumerator Attack() {
                        World_Grid.Combat.HitHere(entity.GetFaction(), posList, 1000);

                        yield return World_Grid.Combat.FlashHere(posList);
                    }
                }
                default:
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds!");
                    break;
            }

            return newTurn;

            void AddToTurnQueue(IEnumerator[] actionArr) {
                Queue<IEnumerator> newTurn = new Queue<IEnumerator>();

                foreach (IEnumerator action in actionArr) { newTurn.Enqueue(action); }

                turnQueue.Enqueue(newTurn);
            }
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!entity) entity = this.GetComponent<Entity>();
    }
}