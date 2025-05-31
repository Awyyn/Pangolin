using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void ResetMushroom()
    {
        gameObject.SetActive(true);
        transform.position = initialPosition;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = true;
    }
}
