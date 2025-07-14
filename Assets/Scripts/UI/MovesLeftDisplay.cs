using UnityEngine;
using TMPro;  // For TextMeshProUGUI

public class MovesLeftDisplay : MonoBehaviour
{
    public TextMeshProUGUI movesLeftText;  // The UI Text element that displays moves left

    // Update the displayed moves left
    public void SetMovesLeft(int moves)
    {
        movesLeftText.text = moves.ToString();  // Update the text in UI
    }
}
