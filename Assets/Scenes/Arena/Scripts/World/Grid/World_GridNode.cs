using UnityEngine;

public class World_GridNode : MonoBehaviour {
    [SerializeField] private bool isPlayerControlled = true;
    
    [SerializeField] private GameObject telegraph;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEngine.UI.Image top, front;
    
    [Header("Node Colors")]
    [SerializeField] private Color color_Player, color_Enemy;

    // Getters
    public Animator GetAnimator() { return animator; }
    public bool IsPlayerControlled() { return isPlayerControlled; }

    // Animations
    // No waiting here as multiple nodes might be performing animations at the same time
    public void StopAnim_Flash() { animator.Play("Grid_Node Idle"); }
    public void PlayAnim_Flash() { animator.Play("Grid_Node Telegraph Flash"); }
    public void PlayAnim_Flicker() { animator.Play("Grid_Node Telegraph Flicker"); }

    // Set isPlayerControlled + update image color accordingly
    public void SetIsPlayerControlled(bool b) {
        // Debug.Log("setIsPlayerControlled: " + b);

        isPlayerControlled = b;
        top.color = front.color = isPlayerControlled ? color_Player : color_Enemy; // Set image according to isPlayerControlled bool
    }

    void Awake() {
        telegraph.SetActive(false);

        if (animator == null) { animator = this.GetComponent<Animator>(); }
    }
}