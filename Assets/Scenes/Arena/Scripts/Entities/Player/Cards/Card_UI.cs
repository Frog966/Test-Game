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

    private Card_Stats card;
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

    // Called by Card_Stats.UpdateUI()
    // Preferably called after Awake()
    public void UpdateText() {
        cost_T.text = card.Cost_Final.ToString();
        dmg_T.text = card.Dmg_Final.ToString() + (card.NoOfHits_Final > 1 ? "x" + card.NoOfHits_Final : "");
    }

    void Awake() {
        card = this.GetComponent<Card_Stats>();
        rectTrans = this.GetComponent<RectTransform>();

        spriteHolder.sprite = card.Image;

        id_T.text = card.ID;
        desc_T.text = card.Desc;
    }
}
