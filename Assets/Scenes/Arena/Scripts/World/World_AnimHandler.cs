using System.Collections;

// Contains a static bool (isAnimating) that is used to determine if any important animations are playing
// Also contains some functions for waiting for animations to finish
public static class World_AnimHandler {
    public static bool isAnimating = false;

    // A simple coroutine that waits for seconds in float
    public static IEnumerator WaitForSeconds(float delay) { yield return new UnityEngine.WaitForSeconds(delay); }
    
    // A simple coroutine that waits for animation to finish playing
    public static IEnumerator WaitForCurrentAnim(UnityEngine.Animator animator) { yield return new UnityEngine.WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); }
}