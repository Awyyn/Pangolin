using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafPile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isFading = false;
    private Vector3 initialPosition;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        initialPosition = transform.position;
        originalColor = spriteRenderer.color;
    }

    public void FadeOut(Vector3 moveToPosition, float moveDuration)
    {
        if (!isFading)
        {
            StartCoroutine(MoveAndFade(moveToPosition, moveDuration));
        }
    }

    private IEnumerator MoveAndFade(Vector3 destination, float duration)
    {
        isFading = true;

        col.enabled = false; // disable collider so player can walk through

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move smoothly
            transform.position = Vector3.Lerp(startPos, destination, t);

            // Fade out
            float alpha = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Ensure fully faded and positioned
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        transform.position = destination;

        //Destroy(gameObject); i am remaking this
        gameObject.SetActive(false);
    }
    public void ResetLeafPile()
    {
        gameObject.SetActive(true);
        isFading = false;
        transform.position = initialPosition;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (col != null)
            col.enabled = true;
    }
}

