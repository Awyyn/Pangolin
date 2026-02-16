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


        PlayerMovement.instance.OnReachedFirefly();
        collected = true;

        StartCoroutine(CollectFireflyRoutine());

    }
    private IEnumerator CollectFireflyRoutine()
    {
        // Flip before animation
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOnLeftSide = screenPos.x < Screen.width / 2f;

        Vector3 scale = transform.localScale;
        scale.x = isOnLeftSide ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Play firefly animation
        animator.SetTrigger("Collect");

        // Start UI animation slightly after (0.7s)
        StartCoroutine(DelayedUIAnimation(0.5f));

        // Complete level immediately
        LevelManager.Instance?.CompleteLevel();

        // Wait for firefly animation to finish
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = state.length; // length of the current animation minus a small buffer
        yield return new WaitForSeconds(waitTime);

        // Destroy firefly after animation
        Destroy(gameObject);

        // Load next level
        LevelManager.Instance?.LoadNextLevelWithDelay(0);
    }

    private IEnumerator DelayedUIAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        FireflyCounterUI.Instance?.PlayCollectAnimation();
    }




}
