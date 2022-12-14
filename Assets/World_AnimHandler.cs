using System.Collections;
using UnityEngine;

// Contains a static bool (isAnimating) that is used to determine if any important animations are playing
// Also contains some functions for waiting for animations to finish
public class World_AnimHandler : MonoBehaviour {
    public static World_AnimHandler Instance { get; private set; }
    public static bool isAnimating { get; set; }

    // A simple coroutine that waits for animation to finish playing
    public IEnumerator WaitForCurrentAnim(Animator animator) { 
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); 
    }

    // A simple coroutine that waits for seconds in float
    public IEnumerator WaitForSeconds(float delay) {
        yield return new WaitForSeconds(delay); 
    }

    void Awake() {
        Instance = this;
        isAnimating = false;
    }
}