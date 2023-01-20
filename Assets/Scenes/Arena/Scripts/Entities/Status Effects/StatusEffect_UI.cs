using UnityEngine;
using UnityEngine.UI;

public class StatusEffect_UI : MonoBehaviour {
    [SerializeField] private IStatusEffect thisSE; // The SE that is attached to this GO

    [Header("UI Stuff")]
    [SerializeField] private Image icon;
    [SerializeField] private Text counter;

    public void SetSprite(Sprite sprite) { icon.sprite = sprite; }
    public void UpdateCounter(int i) { counter.text = i.ToString(); }

    void Awake()  {
        if (thisSE == null) thisSE = this.GetComponent<IStatusEffect>();
        if (thisSE == null) Debug.LogError(this.gameObject.name + " does not have a IStatusEffect!");

        // Trying to cast thisSE as IStatusEffect's inherited interfaces
        IStatusEffect_Timer test1 = thisSE as IStatusEffect_Timer;
        IStatusEffect_Stackable test2 = thisSE as IStatusEffect_Stackable;

        if (test1 != null || test2 != null) {
            counter.gameObject.SetActive(true);

            if (test1 != null) { UpdateCounter(test1.Timer); }
            else { UpdateCounter(test2.Counter); }
        }
        else { counter.gameObject.SetActive(false); }        
    }
}