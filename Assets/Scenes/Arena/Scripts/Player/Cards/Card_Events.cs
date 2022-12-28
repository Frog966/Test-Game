using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Handles how each card behaves under mouse
// Also includes how the card is played
public class Card_Events : EventTrigger {
    private ICard cardScript;

    private static float tweenDur = 0.2f; // Made it static so all copies of this script share the same duration

    private Vector2 mousePosOffset;
    private Vector2 startLocalPos; // Where the card's local pos is in player's hand

    // Setters
    public void SetCardLocalStartPos(Vector2 newPos) { startLocalPos = newPos; } // Sets the pos this card will return to when calling ReturnToHandPos()

    //! Events
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Pointer down event
    public override void OnPointerDown(PointerEventData data) {
        if (!World_AnimHandler.isAnimating) mousePosOffset = (Vector2)GetParentTrans().position - GetMouseWorldPos();
    }

    // Drag event
    public override void OnDrag(PointerEventData data) {
        if (!World_AnimHandler.isAnimating) GetParentTrans().position = GetMouseWorldPos() + mousePosOffset;
    }

    // Drag end event
    public override void OnEndDrag(PointerEventData data) {
        float distance = Vector2.Distance(startLocalPos, GetParentTrans().localPosition);

        Debug.Log("Drag Distance: " + distance);

        if (distance >= 80) { PlayCard(); }
        else { StartCoroutine(ReturnToHandPos()); }
    }

    // Double click event
    public override void OnPointerClick(PointerEventData eventData) {
        if (!World_AnimHandler.isAnimating && eventData.clickCount == 2) { PlayCard(); }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void PlayCard() {
        if (!World_AnimHandler.isAnimating) {
            if (cardScript.EnergyHandler.CanPayEnergyCost(cardScript.Cost)) { cardScript.CardHandler.ResolveCard(cardScript); }
            else { cardScript.EnergyHandler.NotEnoughEnergy(); }
        }
    }
    
    private IEnumerator ReturnToHandPos() {
        World_AnimHandler.isAnimating = true;

        GetParentTrans().DOLocalMove(startLocalPos, tweenDur);

        yield return World_AnimHandler.WaitForSeconds(tweenDur);

        World_AnimHandler.isAnimating = false;
    }

    private Transform GetParentTrans() { return this.transform.parent; }
    private Vector2 GetMouseWorldPos() { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    void Awake() {
        cardScript = GetParentTrans().GetComponent<ICard>();
    }
}