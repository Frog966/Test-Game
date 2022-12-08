using System.Threading.Tasks;
using System.Collections.Generic;

//! This is the base class for enemy AI
//! Do not modify this script unless you're sure you want to affect ALL enemy AIs
interface IEnemyAI {
    List<Task> ReturnCurrentTurn();
}