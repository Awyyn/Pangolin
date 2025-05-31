using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public int movesLeft, initialMovesLeft = 21;  // Default moves (this can be set to any value)
    public MovesLeftDisplay movesLeftDisplay;  // Reference to the UI display

    private void Start()
    {
        UpdateMovesDisplay();         // Initialize the UI
    }

    // Method to add or subtract moves
    public void ModifyMoves(int amount)
    {
        movesLeft += amount;

        if (movesLeft < 0) // Prevent moves from going below 0
        {
            movesLeft = 0;
        }

        UpdateMovesDisplay();
    }

    private void UpdateMovesDisplay() // Update the UI with the new move count
    {
        if (movesLeftDisplay != null)
            movesLeftDisplay.SetMovesLeft(movesLeft);
    }
    public void ResetMoves(int startingMoves) // Reset the moves counter (when restarting the level, for example)
    {
        movesLeft = startingMoves;
        UpdateMovesDisplay();
    }
}
