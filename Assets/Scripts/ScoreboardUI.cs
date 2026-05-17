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
     * Function to display the top score for each level in the Scoreboard Page:
    **/
    public void displayHighScores()
    {
        var playerData = GameManager.Instance.playerData; // Get Global Instance of playerData. 

        StringBuilder newString = new StringBuilder(); // To append each level's of the top score into for display. 

        int overallGameScore = 0; // Overall score from all levels.
        int overallDeathCount = 0; // Overall death count from all levels. 

        // Loop through each Key-Value pair in the Sorted Dictionary starting with the last one (the highest): 
        foreach (var pair in playerData)
        {
            var level = pair.Key; // Outer Key. 
            var levelRecords = pair.Value; // Sorted Dictionary of the level (Inner Key = Score, Inner Value = Death Count)

            if (levelRecords.Count == 0) // If no records exist for this level, skip and go onto next level. 
            {
                continue; 
            }

            int topScore = levelRecords.Keys.Max(); // Grab the highest Score (Inner Key) of the level (Outer Key). 
            int totalDeaths = levelRecords[topScore]; // Value of the Inner Key.

            overallGameScore += topScore; 
            overallDeathCount += totalDeaths; 

            newString.AppendLine("Level: " + level + "    |    " + "Score: " + topScore + "    |    " + "Death Count: " + totalDeaths + "\n"); 
        }

        newString.AppendLine("Total Score: " + overallGameScore + "    |    " + "Total Death Count: " + overallDeathCount); 

        if (scoreText != null) // Safety check to ensure we have the TMPro component for displaying the top scores and death counts.  
        {
            scoreText.text = newString.ToString(); // Set the newly built string to be displayed. 
        }

        //Debug.Log("Successful Score Display!"); 
    }
}