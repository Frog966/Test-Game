// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Game.Unit; // Unique namespace from Game.cs

public class World_Turn : MonoBehaviour {
    private static World_Turns turnsHandler;

    [SerializeField] private UnityEngine.UI.Image bg;

    [Header("UI Stuff")]
    [SerializeField] private GameObject desc_Obj;
    [SerializeField] private TMP_Text desc_Text;
    private string title;

    [Header("Faction Colors")]
    [SerializeField] private Color color_Ally;
    [SerializeField] private Color color_Enemy;
    [SerializeField] private Color color_Neutral;

    // Getters
    public Entity GetOwner() { return owner; }
    public GameObject GetDescObj() { return desc_Obj; }
    public Queue<IEnumerator> GetActionQueue() { return actionQueue; }

    private Entity owner; //! Who this turn belongs to
    public Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();

    //! Basically a constructor. Remember to call this when instantiating a Turn prefab
    // Action queue can be empty especially for player.cs as it does not have defined tasks
    public void Setup(World_Turns _turnsHandler, Entity newOwner, Turn newTurn = null) {
        if (!turnsHandler) { turnsHandler = _turnsHandler; }

        owner = newOwner;

        if (newTurn != null) {
            title = newTurn.title;
            desc_Text.text = newTurn.desc;
            actionQueue = new Queue<IEnumerator>(newTurn.actionQueue);
        }
        else {
            title = null;
            desc_Text.text = null;
            actionQueue = new Queue<IEnumerator>();
        }

        // Set image color
        switch (owner.GetFaction()) {
            case Faction.ALLY:
                bg.color = color_Ally;
                break;
            case Faction.ENEMY:
                bg.color = color_Enemy;
                break;
            default:
                bg.color = color_Neutral;
                break;
        }
    }

    public IEnumerator Execute() {
        Debug.Log(owner.gameObject.name + " performs turn!");

        turnsHandler.DisplayTurnTitle(title);

        while (actionQueue.Count > 0) { yield return actionQueue.Dequeue(); }

        turnsHandler.DisableTurnTitle();

        Debug.Log(owner.gameObject.name + " ends turn!");
    }

    public void EnableDescObj() { desc_Obj.SetActive(true); }
    public void DisableDescObj() { desc_Obj.SetActive(false); }

    void Awake() {
        desc_Obj.SetActive(false);
    }
}