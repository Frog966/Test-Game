using UnityEngine;

public class SE_AttackDown : MonoBehaviour, IStatusEffect_Stackable {
    [SerializeField] private StatusEffect_UI ui;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int counter;
    private Entity entity;

    public StatusEffect_ID ID { get => StatusEffect_ID.ATT_DOWN; }
    public StatusEffect_UI UI { get => ui; } // The UI this SE is attached to
    public Sprite Sprite { get => sprite; }
    public Entity Entity { get => entity; set => entity = value; } // The entity this SE is attached to
    public int Counter { get => counter; set => counter = value; }
    public GameObject GameObject { get => this.gameObject; }

    void Awake() {
        if (!ui) ui = this.GetComponent<StatusEffect_UI>();
    }
}