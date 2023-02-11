using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_Canguard_1 : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;

    private string id = "Canguard 1";
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
            IEnumerator AI() {
                List<Vector2Int> posList = World_Grid.Combat.ReturnPosList_Left(World_Grid.GetEntityGridPos(entity));

                IEnumerator Attack() {
                    List<Entity> hitEntities = World_Grid.Combat.HitHere(entity.GetFaction(), posList, entity.GetFinalDamage(50));

                    yield return World_Grid.Combat.FlashHere(posList);
                }

                if (World_Grid.Combat.AreTheySameRow(entity, Player.GetEntity())) {
                    // ((IEnemy)this).AddToTurnQueue(new Queue<IEnumerator>(new IEnumerator[] { World_Grid.Combat.TelegraphHere(posList) }), "Charging up...");
                    ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { Attack() }, "Fire!");
                    
                    World_Turns.DisplayTurnTitle("Charging up...");

                    yield return World_Grid.Combat.TelegraphHere(posList);
                }
                else {
                    World_Turns.DisplayTurnTitle("Searching...");

                    yield return AnimHandler.WaitForSeconds(0.5f);
                }
            }

            ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { AI() });
            
            return turnQueue.Dequeue(); // Returns the 1st turn of the pattern which is also removed from turn queue
        }
    }

    void Awake() {
        //! Sanity Checks
        if (!entity) entity = this.GetComponent<Entity>();
        if (!animator) animator = this.GetComponent<Animator>();
    }
}