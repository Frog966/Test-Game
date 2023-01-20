using UnityEngine;

public class SE_AttackUp : MonoBehaviour, IStatusEffect_Stackable {
    [SerializeField] private StatusEffect_ID id;
    [SerializeField] private StatusEffect_UI ui;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int counter;
    private Entity entity;

    public StatusEffect_ID ID { get => id; }
    public StatusEffect_UI UI { get => ui; } // The UI this SE is attached to
    public Sprite Sprite { get => sprite; }
    public Entity Entity { get => entity; } // The entity this SE is attached to
    public int Counter { get => counter; set => counter = value; }
    public GameObject GameObject { get => this.gameObject; }

    void Awake() {
        if (!ui) ui = this.GetComponent<StatusEffect_UI>();
    }
}