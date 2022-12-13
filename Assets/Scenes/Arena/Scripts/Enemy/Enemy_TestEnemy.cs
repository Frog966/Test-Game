using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits IEntity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private string id = "Test Enemy";
    private World_Grid gridHandler;
    private int health, healthMax;
    List<List<Task>> turnList = new List<List<Task>>();

    public string ID { get => id; }

    public int Health {
        get => health;
        set => health = value;
    }

    public int HealthMax  {
        get => healthMax;
        set => healthMax = value;
    }

    public Faction Faction { get => Faction.ENEMY; }
    public GameObject GameObj { get => this.gameObject; }
    public World_Grid GridHandler { set => gridHandler = value; }
    
    public List<List<Task>> TurnList { get => turnList; }

    public void OnHit() {}
    public void OnDeath() {}
    //--------------------------------------------------------------------------------------------------------------------------------------

    public List<Task> ReturnCurrentTurn() {
        // Progress pattern
        if (TurnList.Count > 0) {
            Debug.Log("ReturnCurrentTurnTasks 1: " + TurnList.Count);

            TurnList.Remove(TurnList[0]);

            Debug.Log("ReturnCurrentTurnTasks 2: " + TurnList.Count);

            return TurnList[0];
        }
        else {
            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                case 0:
                    return (
                        new List<Task>() {
                            new Task(
                                () => {
                                    Debug.Log("Test Enemy 0");
                                }
                            )
                        }
                    );
                default:
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds!");

                    return new List<Task>();
            }
        }
    }

    void Awake() {
        healthMax = health = 500; // Setting up enemy's health properties
    }
}