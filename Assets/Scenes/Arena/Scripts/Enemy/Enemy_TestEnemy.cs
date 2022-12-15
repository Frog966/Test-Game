using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits IEntity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private string id = "Test Enemy";
    public string ID { get => id; }

    [SerializeField] private int healthMax, health;
    public int Health { get => health; set => health = value; }
    public int HealthMax  { get => healthMax; set => healthMax = value; }

    Queue<Queue<IEnumerator>> turnQueue = new Queue<Queue<IEnumerator>>();
    public Queue<Queue<IEnumerator>> TurnQueue { get => turnQueue; }

    public Faction Faction { get => Faction.ENEMY; }
    public GameObject GameObj { get => this.gameObject; }

    public void OnHit() {}
    public void OnDeath() {}
    //--------------------------------------------------------------------------------------------------------------------------------------

    public Queue<IEnumerator> ReturnCurrentTurn() {
        // Progress pattern
        if (TurnQueue.Count > 0) {
            TurnQueue.Dequeue();

            return TurnQueue.Peek();
        }
        else {
            Queue<IEnumerator> newTurn = new Queue<IEnumerator>();

            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                case 0:
                    newTurn.Enqueue(World_Grid.instance.TelegraphHere(new List<Vector2Int>() { new Vector2Int(0, 1) }));
                    newTurn.Enqueue(World_Grid.instance.FlashHere(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0) }, 0.25f));
                    break;
                default:
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds!");
                    break;
            }

            return newTurn;
        }
    }

    void Awake() {
        healthMax = health = 500; // Setting up enemy's health properties
    }
}