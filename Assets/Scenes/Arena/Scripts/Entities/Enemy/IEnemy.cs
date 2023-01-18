using System.Collections;
using System.Collections.Generic;

//! This is the base interface for enemy scripts
//! Do not modify this script unless you're sure you want to affect ALL enemies
public interface IEnemy {
    // Properties
    string ID { get; }
    Entity Entity { get; } // The entity the AI belongs to. Also used to get any entity-related properties

    // The enemy's AI
    // Returns the current turn's tasks
    Queue<IEnumerator> ReturnCurrentTurn();
}