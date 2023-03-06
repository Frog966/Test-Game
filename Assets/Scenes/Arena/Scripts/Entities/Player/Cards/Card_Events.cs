using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Handles how each card behaves under mouse
// Also includes how the card is played
public class Card_Events : EventTrigger {
    private Canvas canvas;
    private Card_Stats cardScript;

    private Vector2 startLocalPos; // Where the card's local pos is in player's hand
    private Vector2 mousePosOffset;

    private static float tweenDur = 0.2f; // Made it static so all copies of this script share the same duration

    // Setters
    public void SetCardLocalStartPos(Vector2 newPos) { startLocalPos = newPos; } // Sets the pos this card will return to when calling ReturnToHandPos()
    public void SetCanvasOverrideSorting(bool _bool) { canvas.overrideSorting = _bool; }

    //! Events
    // PointerClick events are not overridden as other scripts may want to add their own functions to it
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Pointer down event
    public override void OnPointerDown(PointerEventData data) {
        if (cardScript.isPlayable && !AnimHandler.isAnimating) mousePosOffset = (Vector2)this.transform.position - GetMouseWorldPos();
    }

    // Pointer enter event
    public override void OnPointerEnter(PointerEventData data) {
        if (!AnimHandler.isAnimating) {
            SetCanvasOverrideSorting(true);

            if (cardScript.isPlayable) { 
                this.transform.DOLocalMoveY(cardScript.UIHandler.GetHeight() / 2.0f, 0.1f); 
                
                if (cardScript.EffectHandler != null) { 
                    cardScript.CardHandler.currHoveredCard = cardScript.EffectHandler;
                    cardScript.EffectHandler.DisplayRange(); 
                }
            }
            else { this.transform.DOScale(1.3f, 0.1f); }
        }
    }

    // Pointer exit event
    public override void OnPointerExit(PointerEventData data) {
        if (!AnimHandler.isAnimating) {
            SetCanvasOverrideSorting(false);
            
            if (cardScript.isPlayable) { 
                this.transform.DOLocalMove(startLocalPos, 0.1f); 
                
                if (cardScript.EffectHandler != null) { 
                    if (cardScript.CardHandler.currHoveredCard == cardScript.EffectHandler) { cardScript.CardHandler.currHoveredCard = null; }
                    cardScript.EffectHandler.StopDisplayRange(); 
                }
            }
            else { this.transform.DOScale(1.0f, 0.1f); }
        }
    }

    // Drag event
    public override void OnDrag(PointerEventData data) {
        if (cardScript.isPlayable && !AnimHandler.isAnimating) this.transform.position = GetMouseWorldPos() + mousePosOffset;
    }

    // Drag end event
    public override void OnEndDrag(PointerEventData data) {
        if (cardScript.isPlayable) {
            float distance = Vector2.Distance(startLocalPos, this.transform.localPosition);

            // Debug.Log("Drag Distance: " + distance);

            if (distance >= 80) { PlayCard(); }
            else { StartCoroutine(ReturnToHandPos()); }
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void PlayCard() {
        if (!AnimHandler.isAnimating) {
            if (cardScript.EnergyHandler.CanPayEnergyCost(cardScript.Cost_Final)) { cardScript.CardHandler.ResolveCard(cardScript); }
            else { cardScript.EnergyHandler.NotEnoughEnergy(); }
        }
    }
    
    private IEnumerator ReturnToHandPos() {
        AnimHandler.isAnimating = true;

        this.transform.DOLocalMove(startLocalPos, tweenDur);

        yield return AnimHandler.WaitForSeconds(tweenDur);

        AnimHandler.isAnimating = false;
    }

    private Vector2 GetMouseWorldPos() { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    void Awake() {
        canvas = this.GetComponent<Canvas>(); 
        cardScript = this.GetComponent<Card_Stats>();
        
        SetCanvasOverrideSorting(false);

        // Adding a double click event to this card
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            // Double click event if playable
            if (cardScript.isPlayable && ((PointerEventData)eventData).clickCount == 2 && !AnimHandler.isAnimating) { PlayCard(); }
        });

        this.triggers.Add(entry);
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}