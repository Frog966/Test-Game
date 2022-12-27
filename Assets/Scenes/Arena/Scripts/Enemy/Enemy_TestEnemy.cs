using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits IEntity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    // This AI has patterns that span multiple turns
    private Queue<Queue<IEnumerator>> turnQueue = new Queue<Queue<IEnumerator>>();

    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private string id = "Test Enemy";
    public string ID { get => id; }

    [SerializeField] private int healthMax, health;
    public int Health { get => health; set => health = value; }
    public int HealthMax  { get => healthMax; set => healthMax = value; }

    public Faction Faction { get => Faction.ENEMY; }
    public GameObject GameObj { get => this.gameObject; }

    public void OnHit(int damage) {}
    public void OnDeath() {}
    //--------------------------------------------------------------------------------------------------------------------------------------

    public Queue<IEnumerator> ReturnCurrentTurn() {
        if (turnQueue.Count > 0) { return turnQueue.Dequeue(); }
        else {
            Queue<IEnumerator> newTurn = new Queue<IEnumerator>();

            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                // case 0:
                //     newTurn.Enqueue(World_Grid.TelegraphHere(new List<Vector2Int>() { new Vector2Int(0, 1) }));
                //     newTurn.Enqueue(World_Grid.FlashHere(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0) }, 0.25f));
                //     break;
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
                        World_Grid.Combat.HitHere(this.Faction, posList, 1000);

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
        healthMax = health = 500; // Setting up enemy's health properties
    }
}