using UnityEngine;
using TMPro;  // Needed for TextMeshProUGUI

public class MovesLeftDisplay : MonoBehaviour
{
    public TextMeshProUGUI movesLeftText;

    // Just update the displayed number — no internal logic for moves count here
    public void SetMovesLeft(int moves)
    {
        movesLeftText.text = moves.ToString();
    }
}

