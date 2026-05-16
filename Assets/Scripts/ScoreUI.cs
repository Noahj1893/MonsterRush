// Ethan Le (5/10/2026):
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
using TMPro; 

/** 
 * This script manages the player's score UI.
**/ 
public class ScoreUI : MonoBehaviour 
{
    public TextMeshProUGUI playerScoreboard; 
    public TextMeshProUGUI deathCountUI; 

    // Quit button:
    [SerializeField] Button quitButton; // Assign button via Unity Inspector. 

    // Upon starting the game, call function to initialize default score.  
    void Start()
    {
        CreateUI(); 
    }

    // Ensure we get the text components to display on the Canvas:
    public void CreateUI()
    {
        if (quitButton != null) // Safety check. 
        {
            quitButton.onClick.RemoveAllListeners(); // Remove any potential existing listeners to ensure we do not add extra listeners. 

            // Attach onClick listener to the Quit button:
            quitButton.onClick.AddListener(() => {
                SceneManager.LoadSceneAsync("Title"); // Load main menu when player clicks the Quit button. 
            });
        }

        playerScoreboard.text = "Score: " + 0; // Display the default score of 0. 
        deathCountUI.text = "Death Count: " + 0; // Display the default death count of 0. 
    }

    // Function to update score UI:
    public void UpdateUI(int playerScore, int deathCount)
    {
        if (quitButton != null) // Safety check. 
        {
            quitButton.onClick.RemoveAllListeners(); // Remove any potential existing listeners to ensure we do not add extra listeners. 

            // Attach onClick listener to the Quit button:
            quitButton.onClick.AddListener(() => {
                SceneManager.LoadSceneAsync("Title"); // Load main menu when player clicks the Quit button. 
            });
        }

        playerScoreboard.text = "Score: " + playerScore; // Display the updated score. 
        deathCountUI.text = "Death Count: " + deathCount; // Display the updated death count. 
    }
}