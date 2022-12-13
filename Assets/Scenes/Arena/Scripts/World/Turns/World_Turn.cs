// using System;
using System.Threading.Tasks;
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
    public List<Task> taskList = new List<Task>();

    //! Basically a constructor. Remember to call this when instantiating a Turn prefab
    // Task list can be empty especially for player.cs as it does not have defined tasks
    public void Setup(IEntity newOwner, List<Task> newTaskList = null) {
        owner = newOwner;

        // If newTaskList = null, reset taskList into empty list with no elements
        if (newTaskList != null) { taskList = newTaskList; }
        else { taskList.Clear(); }

        // Set image color
        if (newOwner.GetType().IsAssignableFrom(typeof(Player))) { bg.color = color_Ally; }
        else if (newOwner.GetType().GetInterface(nameof(IEnemy)) != null) { bg.color = color_Enemy; }
        else { bg.color = color_Neutral; }
    }

    // Getters
    public IEntity GetOwner() { return owner; }
    public List<Task> GetTaskList() { return taskList; }

    public void Execute() {
        Debug.Log(owner.GameObj.name + " performs turn!");

        foreach (Task currTask in taskList) { 
            currTask.Start();
            currTask.Wait();
        }

        Debug.Log(owner.GameObj.name + " ends turn!");
    }
}