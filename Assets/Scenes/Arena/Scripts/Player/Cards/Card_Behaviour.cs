using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Handles how each card behaves under mouse
// Also includes how the card is played
public class Card_Behaviour : EventTrigger {
    private ICard cardScript;

    private static float tweenDur = 0.2f; // Made it static so all copies of this script share the same duration

    private Vector2 mousePosOffset;
    private Vector2 startLocalPos; // Where the card's local pos is in player's hand

    //Getters
    public float GetWidthOffset() { return (this.GetComponent<RectTransform>().rect.width / 2.0f); } // Returns half of width because of card's pivot

    // Setters
    public void SetStartLocalPos(Vector2 newPos) { startLocalPos = newPos; }

    // Pointer down event
    public override void OnPointerDown(PointerEventData data) {
        if (!World_AnimHandler.isAnimating) mousePosOffset = (Vector2)this.transform.position - GetMouseWorldPos();
    }

    // Drag event
    public override void OnDrag(PointerEventData data) {
        if (!World_AnimHandler.isAnimating) this.transform.position = GetMouseWorldPos() + mousePosOffset;
    }

    // Drag end event
    public override void OnEndDrag(PointerEventData data) { PlayCard(); }

    // Double click event
    public override void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2 && !World_AnimHandler.isAnimating) {
            if (cardScript.energyHandler.CanPayEnergyCost(cardScript.cost)) { StartCoroutine(MoveToPlayTarget()); }
            else { cardScript.energyHandler.NotEnoughEnergy(); }
        }
    }

    private IEnumerator MoveToPlayTarget() {
        World_AnimHandler.isAnimating = true; // Ends in Player_Cards.PlayCardToGY()

        this.transform.DOLocalMove(this.transform.InverseTransformPoint(cardScript.resolver.GetPlayParent().position) + this.transform.localPosition + new Vector3(GetWidthOffset(), 0.0f, 0.0f), tweenDur);

        yield return World_AnimHandler.WaitForSeconds(tweenDur);
        yield return cardScript.resolver.PlayCardToGY(cardScript); // Play card effect once reach play target
    }
    
    private IEnumerator ReturnToHandPos() {
        World_AnimHandler.isAnimating = true;

        this.transform.DOLocalMove(startLocalPos, tweenDur);

        yield return World_AnimHandler.WaitForSeconds(tweenDur);

        World_AnimHandler.isAnimating = false;
    }

    private void PlayCard() {
        if (!World_AnimHandler.isAnimating) {
            bool canPay = cardScript.energyHandler.CanPayEnergyCost(cardScript.cost);
            float dist = Vector2.Distance(startLocalPos, this.transform.localPosition);

            // Debug.Log("Test: " + startLocalPos + ", " + this.transform.localPosition);
            // Debug.Log("On Drop Dist: " + dist);

            if (canPay && dist >= 80) { StartCoroutine(MoveToPlayTarget()); }
            else {
                if (!canPay) { cardScript.energyHandler.NotEnoughEnergy(); }

                StartCoroutine(ReturnToHandPos()); 
            }
        }
    }

    private Vector2 GetMouseWorldPos() { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    void Awake() {
        if (cardScript == null) cardScript = this.GetComponent<ICard>();
    }
}