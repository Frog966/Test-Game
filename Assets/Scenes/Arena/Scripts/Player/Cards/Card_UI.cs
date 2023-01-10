// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_UI : MonoBehaviour {
    // Card UI
    [SerializeField] private Image spriteHolder;
    [SerializeField] private Text id_T;
    [SerializeField] private Text desc_T;
    [SerializeField] private Text dmg_T;
    [SerializeField] private Text cost_T;

    //Getters
    public float GetWidthOffset() { return (this.GetComponent<RectTransform>().rect.width / 2.0f); } // Returns half of width because of card's pivot

    // Constructor
    public void Setup(Card_Stats card) {
        // Debug.Log("Card_UI Setup: " + card.ID);

        spriteHolder.sprite = card.Image;

        id_T.text = card.ID;
        desc_T.text = card.Desc;
        cost_T.text = card.Cost.ToString();
        dmg_T.text = card.Dmg.ToString() + (card.NoOfHits > 1 ? "x" + card.NoOfHits : "");
    }
}
