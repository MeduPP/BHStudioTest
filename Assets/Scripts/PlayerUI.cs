using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerScoreText;

    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        playerNameText.text = "Player " + newPlayerNumber + ": ";
    }
    public void OnPlayerColorChanged(Color newPlayerColor)
    {
        playerNameText.color = newPlayerColor;
        playerScoreText.color = newPlayerColor;
    }

    public void OnPlayerScoreChanged(ushort newPlayerScore)
    {
        // Show the data in the UI
        playerScoreText.text = newPlayerScore.ToString();
    }
}
