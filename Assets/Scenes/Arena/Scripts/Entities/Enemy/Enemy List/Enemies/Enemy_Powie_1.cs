using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_Powie_1 : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;

    private string id = "Powie 1";
    private Queue<Turn> turnQueue = new Queue<Turn>();

    // Properties
    public string ID { get => id; }
    public Entity Entity { get => entity; }
    public Animator Animator { get => animator; }
    public Queue<Turn> TurnQueue { get => turnQueue; }

    private Vector2Int prevPos;

    // The enemy's AI
    public Turn ReturnCurrentTurn() {
        if (turnQueue.Count > 0) { return turnQueue.Dequeue(); }
        else {
            IEnumerator AI() {
                Vector2Int targetOrigin = World_Grid.GetEntityGridPos(Player.GetEntity());
                List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(
                    targetOrigin,
                    new List<Vector2Int>() { 
                        new Vector2Int(-1, 0),
                        new Vector2Int(1, 0),
                        new Vector2Int(0, -1),
                        new Vector2Int(0, 1)
                    }
                );

                IEnumerator Aim() {
                    ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { Attack() }, "SMASH!!!");

                    World_Turns.DisplayTurnTitle("I see you~~~");

                    yield return World_Grid.Combat.TelegraphHere(posList);
                }

                IEnumerator Attack() {
                    prevPos = World_Grid.GetEntityGridPos(entity);

                    World_Grid.Movement.MoveToPos(entity, targetOrigin);
                    animator.Play("Hard");

                    yield return AnimHandler.WaitForCurrentAnim(animator);

                    List<Entity> hitEntities = World_Grid.Combat.HitHere(entity, posList, 50);

                    yield return World_Grid.Combat.FlashHere(posList);
                    yield return AnimHandler.WaitForSeconds(0.5f);
                    
                    World_Grid.Movement.MoveToPos(entity, prevPos);
                    animator.Play("Idle");
                }
                
                yield return Aim();
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