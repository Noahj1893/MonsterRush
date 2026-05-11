// Ethan Le (5/11/2026):
using System;
using System.Collections.Generic; // For List<>. 
using System.Linq; // For using Reverse() to get top Keys from the Sorted Dictionary.
using System.Text; // For StringBuilder. 
using UnityEngine; 
using UnityEngine.UI; 
using TMPro; 

/** 
 * This script manages the player's overall scoreboard UI.
**/ 
public class ScoreboardUI : MonoBehaviour 
{
    public TextMeshProUGUI scoreText; 

    /** 
     * Function to display the top 5 Scores for each level in the Scoreboard Page:
    **/
    public void displayHighScores()
    {
        var playerData = GameManager.Instance.playerData; // Get Global Instance of playerData. 

        StringBuilder newString = new StringBuilder(); // To append each of the top 5 scores into for display. 

        int i = 0; // Keeps track of how many pairs for each level we have already added to the display. 

        // Loop through each Key-Value pair in the Sorted Dictionary starting with the last one (the highest): 
        foreach (var pair in playerData)
        {
            foreach (var score in pair.Value) // Get each score in the level's Sorted Set. 
            {
                if (i < 5) // Add the level number and its top 5 corresponding scores to the UI:
                {
                    newString.AppendLine("Level: " + pair.Key + "    |    " + "Score: " + score + "\n"); 
                    i++; 
                }

                else // If 5 scores already have been added, reset the tracker and break out of the loop. 
                {
                    newString.AppendLine("\n"); // Extra newline to space out the different levels. 
                    i = 0; 
                    break; 
                }
            }
        }

        if (scoreText != null) // Safety check to ensure we have the TMPro component for displaying the top 5 scores and their death counts.  
        {
            scoreText.text = newString.ToString(); // Set the newly built string to be displayed. 
        }

        //Debug.Log("Successful Score Display!"); 
    }
}