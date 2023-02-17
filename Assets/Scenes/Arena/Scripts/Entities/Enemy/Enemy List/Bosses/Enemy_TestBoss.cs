using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemy interface (which inherits Entity) to Enemy script so that we can use a single type to handle turns
public class Enemy_TestBoss : MonoBehaviour, IEnemy {
    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;
    
    [Header("Audio")]
    [SerializeField] private AudioClip audio_Slash;

    private string id = "Test Boss";
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
            switch(UnityEngine.Random.Range(0, 2)) {
            // switch (1) {
                case 0: {
                    List<List<Vector2Int>> posListList = new List<List<Vector2Int>>();

                    IEnumerator Telegraph(List<List<Vector2Int>> _posListList) {
                        foreach (List<Vector2Int> posList in _posListList) { yield return World_Grid.Combat.TelegraphHere(posList); }
                    }

                    IEnumerator Attack(List<List<Vector2Int>> _posListList) {
                        World_Grid.Movement.MoveToPos(entity, new Vector2Int(4, 1));

                        foreach (List<Vector2Int> posList in _posListList) {
                            List<Entity> hitEntities = World_Grid.Combat.HitHere(entity, posList, 50);

                            yield return World_Grid.Combat.FlashHere(posList);
                        }
                    }

                    Turn ReturnTurn(List<List<Vector2Int>> _posListList) {
                        ((IEnemy)this).AddToTurnQueue(new Queue<IEnumerator>(new IEnumerator[] { Telegraph(_posListList) }), "Telegraphing");
                        ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { Attack(_posListList) }, "Attacking");
                        
                        return turnQueue.Dequeue(); // Returns the 1st turn of the pattern which is also removed from turn queue
                    }

                    switch(UnityEngine.Random.Range(0, 4)) {
                    // switch (3) {
                        case 0: {
                            posListList = new List<List<Vector2Int>> {
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                ),
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(0, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                )
                            };

                            return ReturnTurn(posListList);
                        }
                        case 1: {
                            posListList = new List<List<Vector2Int>> {
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                ),
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(2, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                )
                            };

                            return ReturnTurn(posListList);
                        }
                        case 2: {
                            posListList = new List<List<Vector2Int>> {
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                ),
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 0),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                )
                            };

                            return ReturnTurn(posListList);
                        }
                        case 3: {
                            posListList = new List<List<Vector2Int>> {
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 1),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                ),
                                World_Grid.Combat.ReturnRelativePosList(
                                    new Vector2Int(1, 2),
                                    new List<Vector2Int>() { 
                                        new Vector2Int(-1, 1),
                                        new Vector2Int(1, 1),
                                        new Vector2Int(-1, -1),
                                        new Vector2Int(1, -1)
                                    }
                                )
                            };

                            return ReturnTurn(posListList);
                        }
                        default: {
                            Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds 1!");

                            return null;
                        }
                    }
                }
                case 1: {
                    List<Vector2Int> posList = World_Grid.Combat.ReturnRelativePosList(
                        new Vector2Int(3, 1),
                        new List<Vector2Int>() { 
                            new Vector2Int(-1, 0),
                            new Vector2Int(-1, 1),
                            new Vector2Int(-1, -1),
                            new Vector2Int(-2, 0),
                            new Vector2Int(-2, 1),
                            new Vector2Int(-2, -1),
                        },
                        false
                    );

                    IEnumerator Telegraph() {
                        World_Grid.Movement.MoveToPos(entity, new Vector2Int(3, 1));

                        yield return World_Grid.Combat.TelegraphHere(posList);
                    }

                    IEnumerator Attack() {

                        List<Entity> hitEntities = World_Grid.Combat.HitHere(entity, posList, 100);
                        AudioHandler.PlayClip(audio_Slash);

                        yield return World_Grid.Combat.FlashHere(posList);
                    }

                    ((IEnemy)this).AddToTurnQueue(new Queue<IEnumerator>(new IEnumerator[] { Telegraph() }), "Telegraphing");
                    ((IEnemy)this).AddToTurnQueue(new IEnumerator[] { Attack() }, "Cleaving");
                    
                    return turnQueue.Dequeue(); // Returns the 1st turn of the pattern which is also removed from turn queue
                }
                default: {
                    Debug.LogError(this.gameObject.name + " pattern randomizer went out of bounds 0!");

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