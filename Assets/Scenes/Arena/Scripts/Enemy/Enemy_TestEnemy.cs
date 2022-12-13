using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits IEntity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private string id = "Test Enemy";
    public string ID { get => id; }

    private int health, healthMax;

    public int Health {
        get => health;
        set => health = value;
    }

    public int HealthMax  {
        get => healthMax;
        set => healthMax = value;
    }

    List<List<Task>> turnList = new List<List<Task>>();
    public List<List<Task>> TurnList { get => turnList; }

    public GameObject GameObj { get => this.gameObject; }
    
    private World_Grid gridHandler;
    public World_Grid GridHandler { set => gridHandler = value; }
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
            switch(UnityEngine.Random.Range(0, 4)) {
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
                case 1:
                    return (
                        new List<Task>() {
                            new Task(
                                () => {
                                    Debug.Log("Test Enemy 1");
                                }
                            )
                        }
                    );
                case 2:
                    return (
                        new List<Task>() {
                            new Task(
                                () => {
                                    Debug.Log("Test Enemy 2");
                                }
                            )
                        }
                    );
                case 3:
                    return (
                        new List<Task>() {
                            new Task(
                                () => {
                                    Debug.Log("Test Enemy 3");
                                }
                            )
                        }
                    );
                case 4:
                    return (
                        new List<Task>() {
                            new Task(
                                () => {
                                    Debug.Log("Test Enemy 4");
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