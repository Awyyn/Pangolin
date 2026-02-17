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

        PlayerMovement.instance.NotifyFireflyCollected();

        StartCoroutine(CollectFireflyRoutine());
    }



    private IEnumerator CollectFireflyRoutine()
    {
        // Wait until player finishes movement completely
        while (PlayerMovement.instance.IsMoving())
            yield return null;

        // Now safely trigger look up
        PlayerMovement.instance.PlayLookUpAfterMove();

        // Flip before animation
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOnLeftSide = screenPos.x < Screen.width / 2f;

        Vector3 scale = transform.localScale;
        scale.x = isOnLeftSide ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        animator.SetTrigger("Collect");

        StartCoroutine(DelayedUIAnimation(0.5f));

        LevelManager.Instance?.CompleteLevel();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        Destroy(gameObject);

        LevelManager.Instance?.LoadNextLevelWithDelay(0);
    }




    private IEnumerator DelayedUIAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        FireflyCounterUI.Instance?.PlayCollectAnimation();
    }




}
