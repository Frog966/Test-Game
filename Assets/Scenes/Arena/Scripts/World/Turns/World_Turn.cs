// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Unit; // Unique namespace from Game.cs

public class World_Turn : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Image bg;

    [Header("Faction Colors")]
    [SerializeField] private Color color_Ally;
    [SerializeField] private Color color_Enemy;
    [SerializeField] private Color color_Neutral;

    private IEntity owner; //! Who this turn belongs to
    public Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();

    //! Basically a constructor. Remember to call this when instantiating a Turn prefab
    // Action queue can be empty especially for player.cs as it does not have defined tasks
    public void Setup(IEntity newOwner, Queue<IEnumerator> newTaskList = null) {
        owner = newOwner;

        // If newTaskList = null, reset actionQueue into empty list with no elements
        if (newTaskList != null) { actionQueue = newTaskList; }
        else { actionQueue.Clear(); }

        // Set image color
        if (newOwner.GetType().IsAssignableFrom(typeof(Player))) { bg.color = color_Ally; }
        else if (newOwner.GetType().GetInterface(nameof(IEnemy)) != null) { bg.color = color_Enemy; }
        else { bg.color = color_Neutral; }
    }

    // Getters
    public IEntity GetOwner() { return owner; }
    public Queue<IEnumerator> GetActionQueue() { return actionQueue; }

    public IEnumerator Execute() {
        Debug.Log(owner.GameObj.name + " performs turn!");

        while (actionQueue.Count > 0) {
            yield return actionQueue.Peek();
            
            actionQueue.Dequeue();
        }

        Debug.Log(owner.GameObj.name + " ends turn!");
    }
}