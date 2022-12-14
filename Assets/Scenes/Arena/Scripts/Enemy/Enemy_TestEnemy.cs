using System;
// using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Game.Unit;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits IEntity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    // Properties
    //--------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private string id = "Test Enemy";
    private int health, healthMax;
    List<List<Action>> turnList = new List<List<Action>>();

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
    
    public List<List<Action>> TurnList { get => turnList; }

    public void OnHit() {}
    public void OnDeath() {}
    //--------------------------------------------------------------------------------------------------------------------------------------

    public List<Action> ReturnCurrentTurn() {
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
                        new List<Action>() {
                            new Action(
                                () => {
                                    Debug.Log("Test Enemy 0");

                                    World_Grid.instance.FlashHere(new List<Vector2Int>(){ new Vector2Int(0, 0) });
                                    World_Grid.instance.TelegraphHere(new List<Vector2Int>(){ new Vector2Int(0, 1) });
                                }
                            )
                        }
                    );
                default:
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds!");

                    return new List<Action>();
            }
        }
    }

    void Awake() {
        healthMax = health = 500; // Setting up enemy's health properties
    }
}