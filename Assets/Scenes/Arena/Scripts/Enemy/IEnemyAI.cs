using System.Threading.Tasks;
using System.Collections.Generic;
// using UnityEngine;

//! This is the base class for enemy AI
//! Do not modify this script unless you're sure you want to affect ALL enemy AIs
//! A List<Task> is equivalent to a World_Turn as in a list of tasks is equal to 1 turn
interface IEnemyAI {
    void Setup(World_Grid _gridHandler);
    List<Task> ReturnCurrentTurn();
}