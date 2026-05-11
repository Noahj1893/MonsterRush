// Ethan Le (5/10/2026):
using UnityEngine; 
using UnityEngine.UI; 
using TMPro; 

/** 
 * This script manages the player's score UI.
**/ 
public class ScoreUI : MonoBehaviour 
{
    public TextMeshProUGUI playerScoreboard; 

    // Upon starting the game, call function to initialize default score and speed.  
    void Start()
    {
        CreateUI(); 
    }

    // Upon calling this function after narration, ensure we get the text components to display on the Canvas:
    public void CreateUI()
    {
        playerScoreboard.text = "Score: " + 0; // Display the default score of 0. 
    }

    // Function to update score UI:
    public void UpdateUI(int playerScore)
    {
        playerScoreboard.text = "Score: " + playerScore; // Display the updated score. 
    }
}