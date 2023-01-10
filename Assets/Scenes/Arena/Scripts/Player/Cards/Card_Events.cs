using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Handles how each card behaves under mouse
// Also includes how the card is played
public class Card_Events : EventTrigger {
    private Card_Stats cardScript;

    private Vector2 startLocalPos; // Where the card's local pos is in player's hand
    private Vector2 mousePosOffset;

    private static float tweenDur = 0.2f; // Made it static so all copies of this script share the same duration

    // Setters
    public void SetCardLocalStartPos(Vector2 newPos) { startLocalPos = newPos; } // Sets the pos this card will return to when calling ReturnToHandPos()

    // Constructor
    public void Setup(Card_Stats _cardScript) {
        cardScript = _cardScript;
        
        // Adding a double click event to this card
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            // Double click event if playable
            if (cardScript.cardType == Card_Stats.CardType.PLAYABLE && ((PointerEventData)eventData).clickCount == 2 && !World_AnimHandler.isAnimating) { PlayCard(); }
        });

        this.triggers.Add(entry);
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    //! Events
    // PointerClick events are not overridden as other scripts may want to add their own functions to it
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Pointer down event
    public override void OnPointerDown(PointerEventData data) {
        if (cardScript.cardType == Card_Stats.CardType.PLAYABLE && !World_AnimHandler.isAnimating) mousePosOffset = (Vector2)this.transform.position - GetMouseWorldPos();
    }

    // Drag event
    public override void OnDrag(PointerEventData data) {
        if (cardScript.cardType == Card_Stats.CardType.PLAYABLE && !World_AnimHandler.isAnimating) this.transform.position = GetMouseWorldPos() + mousePosOffset;
    }

    // Drag end event
    public override void OnEndDrag(PointerEventData data) {
        if (cardScript.cardType == Card_Stats.CardType.PLAYABLE) {
            float distance = Vector2.Distance(startLocalPos, this.transform.localPosition);

            // Debug.Log("Drag Distance: " + distance);

            if (distance >= 80) { PlayCard(); }
            else { StartCoroutine(ReturnToHandPos()); }
        }
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

        this.transform.DOLocalMove(startLocalPos, tweenDur);

        yield return World_AnimHandler.WaitForSeconds(tweenDur);

        World_AnimHandler.isAnimating = false;
    }

    private Vector2 GetMouseWorldPos() { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }
}