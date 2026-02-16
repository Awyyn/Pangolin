using UnityEngine;
using System.Collections;

public class FireflyPickup : MonoBehaviour
{

    private Animator animator;
    private bool collected = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;
        if (!collision.CompareTag("Player")) return;

        collected = true;

        // **Immediately mark firefly reached**
        PlayerMovement.instance.reachedFirefly = true;
        PlayerMovement.instance.outOfMovesTriggered = false;

        // Trigger LookUp immediately
        PlayerMovement.instance.animator.ResetTrigger("LookUp");
        PlayerMovement.instance.animator.SetTrigger("LookUp");

        StartCoroutine(CollectFireflyRoutine());

    }


    private IEnumerator CollectFireflyRoutine()
    {
        // Immediately tell the player they reached the firefly
        PlayerMovement.instance.reachedFirefly = true;  // prevent sleep animation
        PlayerMovement.instance.outOfMovesTriggered = false; // just in case

        // Flip before animation
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOnLeftSide = screenPos.x < Screen.width / 2f;

        Vector3 scale = transform.localScale;
        scale.x = isOnLeftSide ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Play firefly animation
        animator.SetTrigger("Collect");

        // Start UI animation slightly after (0.5–0.7s)
        StartCoroutine(DelayedUIAnimation(0.5f));

        // Complete level immediately
        LevelManager.Instance?.CompleteLevel();

        // Wait for firefly animation to finish
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = state.length;
        yield return new WaitForSeconds(waitTime);

        // Destroy firefly after animation
        Destroy(gameObject);

        // Load next level immediately
        LevelManager.Instance?.LoadNextLevelWithDelay(0);
    }



    private IEnumerator DelayedUIAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        FireflyCounterUI.Instance?.PlayCollectAnimation();
    }




}
