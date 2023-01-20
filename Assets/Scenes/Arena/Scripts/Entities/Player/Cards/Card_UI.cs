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

    private RectTransform rectTrans;

    // Getters
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Returns card width
    public float GetWidth() { 
        if (!rectTrans) rectTrans = this.GetComponent<RectTransform>(); // Sanity check

        return rectTrans.rect.width; 
    }

    // Returns card height
    public float GetHeight() { 
        if (!rectTrans) rectTrans = this.GetComponent<RectTransform>(); // Sanity check
        return rectTrans.rect.height; 
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    // Constructor
    public void Setup(Card_Stats card) {
        // Debug.Log("Card_UI Setup: " + card.ID);

        spriteHolder.sprite = card.Image;

        id_T.text = card.ID;
        desc_T.text = card.Desc;
        cost_T.text = card.Cost.ToString();
        dmg_T.text = card.Dmg.ToString() + (card.NoOfHits > 1 ? "x" + card.NoOfHits : "");
    }

    void Awake() {
        rectTrans = this.GetComponent<RectTransform>();
    }
}
