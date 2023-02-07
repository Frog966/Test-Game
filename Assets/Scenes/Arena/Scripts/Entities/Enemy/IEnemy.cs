using System.Collections;
using System.Collections.Generic;

//! This is the base interface for enemy scripts
//! Do not modify this script unless you're sure you want to affect ALL enemies
public interface IEnemy {
    // Properties
    string ID { get; }
    Entity Entity { get; } // The entity the AI belongs to. Also used to get any entity-related properties

    // Turn queue stuff
    // Just defining functions for consistency between enemies
    //-----------------------------------------------------------------------------------------------------------------------------------------------
    // Some enemies have patterns that last multiple turns so this queue is to keep track of that
    //! You do not need to use TurnQueue. You only need to return a Turn using ReturnCurrentTurn()
    Queue<Turn> TurnQueue { get; } 

    public void AddToTurnQueue(IEnumerator[] actionArr, string title = null, string desc = null) {
        AddToTurnQueue(new Queue<IEnumerator>(actionArr), title, desc);
    }
    
    public void AddToTurnQueue(Queue<IEnumerator> actionQueue, string title = null, string desc = null) {
        TurnQueue.Enqueue(new Turn(actionQueue, title, desc));
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------

    // The enemy's AI
    // Returns the current turn's tasks
    Turn ReturnCurrentTurn();
}