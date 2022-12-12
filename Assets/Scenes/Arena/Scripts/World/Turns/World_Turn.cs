// using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using Game.Unit; // Unique namespace from Game.cs

public class World_Turn : MonoBehaviour {
    private IEntity owner; //! Who this turn belongs to
    public List<Task> taskList = new List<Task>();

    //! Basically a constructor. Remember to call this when instantiating a Turn prefab
    // Task list can be empty especially for player.cs as it does not have defined tasks
    public void Setup(IEntity newOwner, List<Task> newTaskList = null) {
        owner = newOwner;
        // taskList = newTaskList != null ? newTaskList : new List<Task>(); // If newTaskList = null, reset taskList into empty list with no elements

        // If newTaskList = null, reset taskList into empty list with no elements
        if (newTaskList != null) { taskList = newTaskList; }
        else { taskList.Clear(); }
    }

    // Getters
    public IEntity GetOwner() { return owner; }
    public List<Task> GetTaskList() { return taskList; }
    public void SetTaskList(List<Task> newTaskList) { taskList = newTaskList; }

    public void Execute() {
        foreach (Task currTask in taskList) { 
            currTask.Start();
            currTask.Wait();
        }
    }
}