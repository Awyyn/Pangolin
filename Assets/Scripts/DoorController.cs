using UnityEngine;

public class DoorController : MonoBehaviour
{
    public PlateIndicator[] indicators; // assign via inspector
    public int requiredPlates;
    public Animator doorAnimator;
    public Animator dustAnimator;
    public Collider2D doorCollider; // assign the collider of the door here

    private int pressedCount = 0;

    private void Awake()
    {
        if (doorAnimator == null)
            doorAnimator = GetComponent<Animator>();

        if (doorCollider == null)
            doorCollider = GetComponent<Collider2D>();
    }

    public void PlatePressed(PressurePlate plate, bool pressed)
    {
        pressedCount += pressed ? 1 : -1;
        pressedCount = Mathf.Clamp(pressedCount, 0, requiredPlates); // ensure it doesn't go negative or exceed indicators in case more Indicators fire at the same time
        UpdateIndicators();
        UpdateDoorState();
    }

    private void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i].SetActive(i < pressedCount);
        }
    }

    private void UpdateDoorState()
    {
        bool shouldBeOpen = pressedCount >= requiredPlates;
        doorAnimator.SetBool("Open", shouldBeOpen);
        dustAnimator.SetBool("Open", shouldBeOpen);

        if (doorCollider != null)
            doorCollider.enabled = !shouldBeOpen; // disable collider when door is open
    }
    public void ResetState()
    {
        // Reset counters and indicators, then close the door
        pressedCount = 0;
        UpdateIndicators();
        SetOpen(false);
    }

    private void SetOpen(bool open)
    {
        if (doorAnimator != null) doorAnimator.SetBool("Open", open);
        if (dustAnimator != null) dustAnimator.SetBool("Open", open);
        if (doorCollider != null) doorCollider.enabled = !open;
    }

}





