using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! This is the base interface for enemy scripts
//! Do not modify this script unless you're sure you want to affect ALL enemies
public interface IEnemy : IEntity {
    // Properties
    string ID { get; }
    Queue<Queue<IEnumerator>> TurnQueue { get; } // Contains a chain of turns in the event the enemy does something that requires specific actions over multiple turns

    Queue<IEnumerator> ReturnCurrentTurn();  // Returns the current turn's tasks
}