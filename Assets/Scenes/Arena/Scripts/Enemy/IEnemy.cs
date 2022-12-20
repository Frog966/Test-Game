using System.Collections;
using System.Collections.Generic;

//! This is the base interface for enemy scripts
//! Do not modify this script unless you're sure you want to affect ALL enemies
public interface IEnemy : IEntity {
    // Properties
    string ID { get; }

    Queue<IEnumerator> ReturnCurrentTurn();  // Returns the current turn's tasks
}