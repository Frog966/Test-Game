using UnityEngine;
using UnityEngine.UI;

public class World_BattleRewards_Button : MonoBehaviour {
    [SerializeField] private Button button;
    [SerializeField] private Text text;
    [SerializeField] private Image icon;
    
    [Header("Icons")]
    [SerializeField] private Sprite icon_Bits;
    [SerializeField] private Sprite icon_Cards;

    // Getters
    public Button GetButton() { return button; }

    public void Setup(RewardType rewardType, int coinReward = 0) {
        //! Sanity Checks
        if (!button) button = this.GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        switch(rewardType) {
            case RewardType.BITS: 
                icon.sprite = icon_Bits;
                text.text = coinReward + " coins";
            break;
            default: 
                icon.sprite = icon_Cards;
                text.text = "Get new cards!";
            break;
        }
    }
}
