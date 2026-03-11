using System.Collections;
using UnityEngine;

public class LeafPile : MonoBehaviour, IInteractable
{
    public Animator animator;
    private Collider2D col;
    private bool hasInteracted = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Interact(Vector3 playerDirection)
    {
        Debug.Log("[LeafPile] Interacted with!");
        if (hasInteracted)
            return;

        hasInteracted = true;

        // Disable collider so it won't trigger again
        if (col != null)
            col.enabled = false;

        // Determine animation based on player direction
        PlayFlowAnimation(playerDirection);
    }

    private void PlayFlowAnimation(Vector3 dir)
    {
        if (animator == null)
        {
            Debug.LogWarning("[LeafPile] Animator missing!");
            return;
        }

        Debug.Log("[LeafPile] Playing animation based on direction: " + dir);

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
            {
                animator.Play("LeafpileFlowUp");
                Debug.Log("Played LeafpileFlowUp");
            }
            else
            {
                animator.Play("LeafpileFlowDown");
                Debug.Log("Played LeafpileFlowDown");
            }
        }
        else
        {
            animator.Play("LeafpileFlowSide");
            animator.transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
            Debug.Log("Played LeafpileFlowSide, flipped: " + (dir.x > 0 ? "no" : "yes"));
        }
    }

    public void ResetState()
    {
        hasInteracted = false;

        if (col != null)
            col.enabled = true;

        // Reset animator to idle/default state
        if (animator != null)
        {
            animator.Play("LeafIdle");
            animator.transform.localScale = new Vector3(1, 1, 1); // reset flipping
        }

        // Ensure the LeafPile GameObject is active
        gameObject.SetActive(true);
    }
}
