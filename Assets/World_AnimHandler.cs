using System.Collections;
using UnityEngine;

// Contains a static bool that is used to determine if any important animations are playing
// Also contains some functions for waiting for the animation to finish
public class World_AnimHandler : MonoBehaviour {
    public static World_AnimHandler Instance { get; private set; }
    public static bool isAnimating = false;

    // A simple coroutine that waits for animation to finish playing
    public IEnumerator WaitForCurrentAnim(Animator animator) { 
        isAnimating = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); 
        
        isAnimating = false;
    }

    // A simple coroutine that waits for seconds
    public IEnumerator WaitForSeconds(float delay) { 
        isAnimating = true;

        yield return new WaitForSeconds(delay); 
        
        isAnimating = false;
    }

    void Awake() {
        Instance = this;
    }
}