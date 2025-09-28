using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Animator animator; // Press/Unpress animation
    public DoorController linkedDoor;

    private int pressers = 0; // track how many objects are pressing the plate

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidPresser(other)) return;

        pressers++;
        if (pressers == 1) // only trigger on first presser
        {
            animator.SetBool("Pressed", true);
            linkedDoor?.PlatePressed(this, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidPresser(other)) return;

        pressers = Mathf.Max(pressers - 1, 0);
        if (pressers == 0) // only trigger when no more objects pressing
        {
            animator.SetBool("Pressed", false);
            linkedDoor?.PlatePressed(this, false);
        }
    }

    private bool IsValidPresser(Collider2D other)
    {
        return other.CompareTag("Player") || other.CompareTag("Rock");
    }

    public void ResetState()
    {
        pressers = 0;         // reset internal counter and animator
        if (animator != null) animator.SetBool("Pressed", false);

        linkedDoor?.PlatePressed(this, false); // notify linked door that this plate is not pressed anymore
    }

}
