using UnityEngine;
using UnityEngine.UI;

public class PlateIndicator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color activeColor = Color.blue;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // faded gray

    public void SetActive(bool active)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = active ? activeColor : inactiveColor;
    }
}
