using System.Linq;
using System.Collections;
using UnityEngine;

public class Card_Push : MonoBehaviour, ICardEffect {
    [SerializeField] private Card_Stats cardStats;

    // Do not call Effect(). Card_Events will call it instead
    // Does not require AnimHandler.isAnimating as Card_Events will handle that
    public IEnumerator Effect() {
        Debug.Log(this + " is being played!");

        // An entity list which is sorted by their x pos in ascending order
        System.Collections.Generic.List<Entity> entitiesList = 
            World_Grid.GetAllEntities()
            .Where((entity) => entity.GetFaction() == Game.Unit.Faction.ENEMY || entity.GetFaction() == Game.Unit.Faction.NEUTRAL)
            .OrderByDescending((entity) => World_Grid.GetEntityGridPos(entity).x)
            .ToList();
        
        foreach (Entity entity in entitiesList) {
            Vector2Int currPos = World_Grid.GetEntityGridPos(entity);
            Vector2Int newPos = new Vector2Int(currPos.x + 1, currPos.y);
            World_GridNode newNode = World_Grid.GetNode(newPos);

            // Do not move enemy into player-controlled node
            if (newNode && !newNode.IsPlayerControlled()) { World_Grid.Movement.MoveToPos(entity, newPos); }
        }

        yield return null;
    }

    void Awake() {
        if (!cardStats) cardStats = this.GetComponent<Card_Stats>();
    }
}