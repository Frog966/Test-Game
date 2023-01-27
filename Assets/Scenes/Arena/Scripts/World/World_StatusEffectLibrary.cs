using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//! Contains all status effects
public class World_StatusEffectLibrary : MonoBehaviour {
    [SerializeField] private Player player;

    [SerializeField] private Transform statusEffectPool;
    public static Transform StatusEffectPool { get => inst.statusEffectPool; }

    private static World_StatusEffectLibrary inst; // A private instance of this script just to get the static functions to work
    private static Dictionary<StatusEffect_ID, IStatusEffect> library = new Dictionary<StatusEffect_ID, IStatusEffect>();

    public static void AddStatusEffect(Entity target, StatusEffect_ID id, int stackChange = 1) {
        IStatusEffect se = target.statusEffect_List.Find((el) => el.ID.Equals(id));
        IStatusEffect_Stackable stackScript = se as IStatusEffect_Stackable;
        
        World_StatusEffectLibrary _this = World_StatusEffectLibrary.inst;

        // Check if the stackable entity already has the SE
        // If the entity doesn't have the SE is receiving a timer SE, instatiate a new one
        if (se != null && stackScript != null) { 
            // If entity receives the same stackable status effect, increase it by stackChange. stackChange can be negative
            stackScript.AddCounter(stackChange); 
        }
        else { // Add new SE
            Transform newSE = null;

            // Try to find a already instantiated SE from pool first
            foreach (Transform child in inst.statusEffectPool) {
                if (child.GetComponent<IStatusEffect>().ID == id) {
                    newSE = child;
                    break; // Break the loop once child is found
                }
            }

            // If cannot find an SE from pool, instantiate a deep copy from library into pool
            if (!newSE)  { newSE = GameObject.Instantiate(library[id].GameObject, _this.statusEffectPool).transform; }

            // Setup new SE
            //-----------------------------------------------------------------------------------------------------------------------------------------
            IStatusEffect newSEScript = newSE.GetComponent<IStatusEffect>();
            IStatusEffect_Stackable newStackScript = newSEScript as IStatusEffect_Stackable;

            newSEScript.Entity = target;

            // Set new stackable SE's counter
            if (newStackScript != null) { 
                newStackScript.Counter = 0; 
                newStackScript.AddCounter(stackChange); 
            }

            target.statusEffect_List.Add(newSEScript); // Add the new SE into the target's SE list

            newSE.GetComponent<StatusEffect_UI>().SetSprite(newSEScript.Sprite);
            newSE.SetParent(target.GetSEParent());
            newSE.localScale = Vector3.one;
            //-----------------------------------------------------------------------------------------------------------------------------------------
        }

        // Depending on the SE, update player cards' UI
        if (target == _this.player.GetEntity() && id == StatusEffect_ID.ATTACK) { _this.player.CardsHandler().UpdateHandCardUI(); }
    }

    public static void AddStatusEffect(List<Entity> targets, StatusEffect_ID id, int stackChange = 0) {
        foreach (Entity target in targets) { AddStatusEffect(target, id, stackChange); }
    }

    public static void AddStatusEffect(List<Entity> targets, List<StatusEffect_ID> ids, int stackChange = 0) {
        foreach (StatusEffect_ID id in ids) {
            foreach (Entity target in targets) { AddStatusEffect(target, id, stackChange); }
        }
    }

    void Awake() {
        // Instance declaration
        if (inst != null && inst != this) { Destroy(this); }
        else { inst = this; }

        // Sanity checks
        if (!player) { Debug.LogError("World_StatusEffectLibrary does not have Player.cs!"); }

        statusEffectPool.gameObject.SetActive(false);
        
        foreach (IStatusEffect se in Resources.LoadAll("Status Effects", typeof(IStatusEffect)).Cast<IStatusEffect>().ToArray()) {
            if (!library.ContainsKey(se.ID)) { library.Add(se.ID, se); }
            else { Debug.LogWarning("There is a duplicate status effect ID '" + se.ID + "' found in the Resources folder!"); }
        }
    }
}

// An enum that holds every status effect (buff/debuff)
public enum StatusEffect_ID {
    ATTACK,
    DEFENCE,
    STUN
}