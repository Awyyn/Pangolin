using UnityEngine;

public class Anthill : MonoBehaviour, IInteractable
{
    private bool hasTriggered = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Interact(Vector3 direction)
    {
        Debug.Log("[Anthill] Interacted with!");

        if (hasTriggered)
            return;

        if (MovesManager.Instance != null)
        {
            MovesManager.Instance.ModifyMoves(+3);
        }

        hasTriggered = true;

        // Optional: visually disable after use
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;
    }

    public void ResetState()
    {
        hasTriggered = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = true;

        Debug.Log("[Anthill] State reset.");
    }
}
