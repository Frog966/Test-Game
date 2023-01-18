using System.Collections.Generic;
using UnityEngine;

//! Contains all status effects
public class World_StatusEffectLibrary : MonoBehaviour {
    [SerializeField] private GameObject prefab;
    [SerializeField] private StatusEffect_Info[] statusEffectList = new StatusEffect_Info[System.Enum.GetValues(typeof(StatusEffect_ID)).Length];

    private static Dictionary<StatusEffect_ID, StatusEffect_Info> dictionary = new Dictionary<StatusEffect_ID, StatusEffect_Info>();

    void Awake() {
        foreach (StatusEffect_Info info in statusEffectList) {
            if (dictionary.ContainsKey(info.id)) { Debug.LogWarning("Duplicate status effect key '" + info.id + "' found!"); }
            else { dictionary[info.id] = info; }
        }
    }

    public static void AddStatusEffect(Entity target, StatusEffect_ID id) {
        // target.statusEffect_List;
    }

    public static void AddStatusEffect(List<Entity> targets, StatusEffect_ID id) {
        foreach (Entity target in targets) { AddStatusEffect(target, id); }
    }
}

// A class that contains a status effect's info
//! Does not actually contain a status effect's functionality as player and enemies use them differently
[System.Serializable]
public class StatusEffect_Info {
    public bool isStackable;
    public StatusEffect_ID id;
    public Sprite sprite;
    
    private int stackCounter;

    private StatusEffect_UI ui;

    // Constructor
    public StatusEffect_Info(StatusEffect_UI newUI) {
        ui = newUI;
        ui.Setup(this);
    }

    public void SetStackCounter(int i) {
        stackCounter = i;
        ui.UpdateCounter();
    }
}

// An enum that holds every status effect (buff/debuff)
public enum StatusEffect_ID {
    ATTACK_UP,
    ATTACK_DOWN,
    DEF_UP,
    DEF_DOWN,
    STUN
}