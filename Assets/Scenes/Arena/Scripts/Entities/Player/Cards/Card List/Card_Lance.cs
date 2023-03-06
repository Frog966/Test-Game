using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Lance : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;
        
    List<Vector2Int> posList;

    public void DisplayRange() { 
        posList = 
            World_Grid.Combat.ReturnRelativePosList(
                World_Grid.GetEntityGridPos(Player.GetEntity()), 
                new List<Vector2Int>() { 
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                },
                false
            );

        World_Grid.Combat.FlashHere_Start(posList); 
    }

    public void StopDisplayRange() { 
        posList = 
            World_Grid.Combat.ReturnRelativePosList(
                World_Grid.GetEntityGridPos(Player.GetEntity()), 
                new List<Vector2Int>() { 
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                },
                false
            );
            
        World_Grid.Combat.FlashHere_Stop(posList); 
    }

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");
        
        posList = 
            World_Grid.Combat.ReturnRelativePosList(
                World_Grid.GetEntityGridPos(Player.GetEntity()), 
                new List<Vector2Int>() { 
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                },
                false
            );
        
        Player.GetEntity().PlayAnimation("Punch Start");
        
        yield return AnimHandler.WaitForSeconds(0.2f);

        Player.GetEntity().PlayAnimation("Punch End");

        World_Grid.Combat.HitHere(Player.GetEntity(), posList, cardStats.Dmg_Base);

        yield return World_Grid.Combat.FlashHere(posList, 0.1f);
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}