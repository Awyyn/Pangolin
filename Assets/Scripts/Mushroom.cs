using UnityEngine;

public class Mushroom : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Interact(Vector3 direction)
    {
        FindObjectOfType<MovesManager>().ModifyMoves(-2);
        spriteRenderer.enabled = false;
        col.enabled = false;
    }

    public void ResetState()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        if (col != null)
            col.enabled = true;
    }

}
