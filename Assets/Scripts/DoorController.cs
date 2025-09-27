using UnityEngine;

public class DoorController : MonoBehaviour
{
    public PlateIndicator[] indicators; // assign via inspector
    public int requiredPlates;
    public Animator doorAnimator;
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
        pressedCount = Mathf.Clamp(pressedCount, 0, indicators.Length); // ensure it doesn't go negative or exceed indicators in case more Indicators fire at the same time
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

        if (doorCollider != null)
            doorCollider.enabled = !shouldBeOpen; // disable collider when door is open
    }
}





