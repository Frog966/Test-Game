using UnityEngine;
using UnityEngine.UI;

public class StatusEffect_UI : MonoBehaviour {
    [SerializeField] private StatusEffect_Info info;
    
    [Header("UI Stuff")]
    [SerializeField] private Image icon;
    [SerializeField] private Text stackCounter;

    // Constructor
    public void Setup(StatusEffect_Info newInfo) {
        icon.sprite = newInfo.sprite;

        if (newInfo.isStackable) {
            UpdateCounter();
            stackCounter.gameObject.SetActive(true);
        }
        else {
            stackCounter.gameObject.SetActive(false);
        }
    }

    public void UpdateCounter() { stackCounter.text = info.ToString(); }
}