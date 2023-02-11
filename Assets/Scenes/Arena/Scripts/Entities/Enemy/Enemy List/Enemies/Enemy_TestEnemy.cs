using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestEnemy : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;

    private string id = "Test Enemy";
    private Queue<Turn> turnQueue = new Queue<Turn>();

    // Properties
    public string ID { get => id; }
    public Entity Entity { get => entity; }
    public Animator Animator { get => animator; }
    public Queue<Turn> TurnQueue { get => turnQueue; }

    // The enemy's AI
    public Turn ReturnCurrentTurn() {
        if (turnQueue.Count > 0) { return turnQueue.Dequeue(); }
        else {
            // This switch case contains every pattern under this AI
            switch(UnityEngine.Random.Range(0, 1)) {
                case 0: {
                    List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(
                        new Vector2Int(1, 1),
                        new List<Vector2Int>() { 
                            new Vector2Int(-1, 0),
                            new Vector2Int(1, 0),
                            new Vector2Int(0, -1),
                            new Vector2Int(0, 1)
                        }
                    );

                    IEnumerator Attack() {
                        List<Entity> hitEntities = World_Grid.Combat.HitHere(entity.GetFaction(), posList, entity.GetFinalDamage(10));

                        yield return World_Grid.Combat.FlashHere(posList);

                        if (hitEntities.Count > 0) {
                            World_StatusEffectLibrary.AddStatusEffect(entity, StatusEffect_ID.ATTACK, 2);
                        }
                    }

                    ((IEnemy)this).AddToTurnQueue(new Queue<IEnumerator>(new IEnumerator[] { World_Grid.Combat.TelegraphHere(posList) }), "Telegraphing", "Test 1");
                    ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { Attack() }, "Attacking", "Test 2");
                    
                    return turnQueue.Dequeue(); // Returns the 1st turn of the pattern which is also removed from turn queue
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
        if (!animator) animator = this.GetComponent<Animator>();
    }
}