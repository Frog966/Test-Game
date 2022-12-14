using UnityEngine;

public class World_GridNode : MonoBehaviour {
    [SerializeField] private bool isPlayerControlled = true;
    
    [SerializeField] private GameObject telegraph;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEngine.UI.Image top, front;
    
    [Header("Node Colors")]
    [SerializeField] private Color color_Player, color_Enemy;

    // Getters
    public bool IsPlayerControlled() { return isPlayerControlled; }

    // Animations
    // No waiting here as multiple nodes might be performing animations at the same time
    public void PlayAnim_Flash() { animator.SetBool("isFlash", true); }
    public void StopAnim_Flash() { animator.SetBool("isFlash", false); }
    public void PlayAnim_Flicker() { animator.SetBool("isFlicker", true); }
    public void StopAnim_Flicker() { animator.SetBool("isFlicker", false); }

    // Set isPlayerControlled + update image color accordingly
    public void SetIsPlayerControlled(bool b) {
        // Debug.Log("setIsPlayerControlled: " + b);

        isPlayerControlled = b;
        top.color = front.color = isPlayerControlled ? color_Player : color_Enemy; // Set image according to isPlayerControlled bool
    }

    void Awake() {
        telegraph.SetActive(false);

        if (animator == null) this.GetComponent<Animator>();
    }
}