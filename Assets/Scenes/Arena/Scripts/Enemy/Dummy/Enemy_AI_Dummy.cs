using UnityEngine;

public class Enemy_AI_Dummy : Enemy_AI {
    // Defining an abstract Act() from base Enemy_AI script
    public override void Act() {
        Debug.Log("I'm a dummy!");
    }
}