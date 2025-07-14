using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public int movesLeft;
    public MovesLeftDisplay movesLeftDisplay;

    public static MovesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetMoves(int startingMoves)
    {
        movesLeft = startingMoves;
        UpdateMovesDisplay();
    }

    // Modify moves left (subtract or add)
    public void ModifyMoves(int amount)
    {
        movesLeft += amount;

        if (movesLeft < 0)
        {
            movesLeft = 0; // Prevent negative moves
        }

        UpdateMovesDisplay();
    }


    public void UpdateMovesDisplay()
    {
        if (movesLeftDisplay != null)
        {
            movesLeftDisplay.SetMovesLeft(movesLeft);  // Update UI text with moves left
        }
    }
}
