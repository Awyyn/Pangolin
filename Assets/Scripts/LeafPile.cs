using System.Collections;
using UnityEngine;

public class LeafPile : MonoBehaviour, IInteractable
{
    private Animator animator;
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
        if (animator == null) return;

        if (dir == Vector3.up)
        {
            animator.Play("LeafpileFlowUp");
        }
        else if (dir == Vector3.down)
        {
            animator.Play("LeafpileFlowDown");
        }
        else if (dir == Vector3.left)
        {
            animator.Play("LeafpileFlowSide");
            animator.transform.localScale = new Vector3(-1, 1, 1); // flip horizontally
        }
        else // Vector3.right or default
        {
            animator.Play("LeafpileFlowSide");
            animator.transform.localScale = new Vector3(1, 1, 1); // normal
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
            animator.Play("Idle"); // optional: make a default idle animation
            animator.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
