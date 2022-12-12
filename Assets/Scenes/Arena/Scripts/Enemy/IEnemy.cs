using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

//! This is the base interface for enemy scripts
//! Do not modify this script unless you're sure you want to affect ALL enemies
public interface IEnemy : IEntity {
    // Properties
    string ID { get; }
    World_Grid GridHandler { set; }
    List<List<Task>> TurnList { get; } // Contains a chain of turns in the event the enemy does something that requires specific actions over multiple turns

    List<Task> ReturnCurrentTurn();  // Returns the current turn's tasks
}