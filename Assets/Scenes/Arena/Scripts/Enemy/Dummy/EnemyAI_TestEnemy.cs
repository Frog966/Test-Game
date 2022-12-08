using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// C# does not allow multiple inheritance so we're attaching an IEnemyAI interface to this script so that we can use a single type to access the enemy's AI from Enemy.cs
public class EnemyAI_TestEnemy : MonoBehaviour, IEnemyAI {
    // Handlers
    private World_Grid gridHandler;

    // Contains a chain of turns in the event the enemy does something that requires specific actions over turns
    private List<List<Task>> turnList = new List<List<Task>>();

    public void Setup(World_Grid _gridHandler) { gridHandler = _gridHandler; }

    private List<Task> ReturnRandomMove() {
        return new List<Task>();
    }

    public List<Task> ReturnCurrentTurn() {
        if (turnList.Count > 0) {
            List<Task> nextTurn = turnList[0];

            turnList.Remove(nextTurn);

            return nextTurn;
        }
        else {
            return ReturnRandomMove();
        }
    }
}