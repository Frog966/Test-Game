using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//! Contains all status effects
public class World_StatusEffectLibrary : MonoBehaviour {
    [SerializeField] private Transform statusEffectPool;
    public static Transform StatusEffectPool { get => inst.statusEffectPool; }

    private static World_StatusEffectLibrary inst; // A private instance of this script just to get the static functions to work
    private static Dictionary<StatusEffect_ID, IStatusEffect> library = new Dictionary<StatusEffect_ID, IStatusEffect>();

    public static void AddStatusEffect(Entity target, StatusEffect_ID id) {
        IStatusEffect se = target.statusEffect_List.Find((el) => el.ID.Equals(id));
        IStatusEffect_Stackable stackScript = se as IStatusEffect_Stackable;

        // Check if the stackable entity already has the SE
        // If the entity doesn't have the SE is receiving a timer SE, instatiate a new one
        if (se != null && stackScript != null) { stackScript.AddCounter(); } // If entity receives the same stackable status effect, increase it by 1
        else {
            World_StatusEffectLibrary _this = World_StatusEffectLibrary.inst;
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

            IStatusEffect newSEScript = newSE.GetComponent<IStatusEffect>();
            newSEScript.Entity = target;
            target.statusEffect_List.Add(newSEScript); // Add the new SE into the target's SE list

            newSE.GetComponent<StatusEffect_UI>().SetSprite(newSEScript.Sprite);
            newSE.SetParent(target.GetSEParent());
            newSE.localScale = Vector3.one;
        }
    }

    public static void AddStatusEffect(List<Entity> targets, StatusEffect_ID id) {
        foreach (Entity target in targets) { AddStatusEffect(target, id); }
    }

    public static void AddStatusEffect(List<Entity> targets, List<StatusEffect_ID> ids) {
        foreach (StatusEffect_ID id in ids) {
            foreach (Entity target in targets) { AddStatusEffect(target, id); }
        }
    }

    void Awake() {
        // Instance declaration
        if (inst != null && inst != this) { Destroy(this); }
        else { inst = this; }

        statusEffectPool.gameObject.SetActive(false);
        
        foreach (IStatusEffect se in Resources.LoadAll("Status Effects", typeof(IStatusEffect)).Cast<IStatusEffect>().ToArray()) {
            if (!library.ContainsKey(se.ID)) { library.Add(se.ID, se); }
            else { Debug.LogWarning("There is a duplicate status effect ID '" + se.ID + "' found in the Resources folder!"); }
        }
    }
}

// An enum that holds every status effect (buff/debuff)
public enum StatusEffect_ID {
    ATT_UP,
    ATT_DOWN,
    DEF_UP,
    DEF_DOWN,
    STUN
}