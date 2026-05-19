using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject levelsPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject scoreboardPanel; 
    [SerializeField] GameObject creditsPanel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] private string[] storyLines; // For pre-level 1 narration. 
    [SerializeField] private StoryController storyController; // For pre-level 1 narration.

    public void ShowMenu()
    {
        audioSource.Play();
        menuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        scoreboardPanel.SetActive(false); 
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    public void ShowLevels()
    {
        audioSource.Play();
        menuPanel.SetActive(false);
        levelsPanel.SetActive(true);
        scoreboardPanel.SetActive(false); 
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    public void ShowSettings()
    {
        audioSource.Play();
        menuPanel.SetActive(false);
        levelsPanel.SetActive(false);
        scoreboardPanel.SetActive(false); 
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void LoadScene(int level)
    {
        audioSource.Play();
        StartCoroutine(LoadSceneDelayed(level));
    }
    IEnumerator LoadSceneDelayed(int level)
    {
        yield return new WaitForSeconds(0.2f);
        if(level < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            if (level > 1) // No beginning narration if it is not level 1. 
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level);
            }

            else // If level 1, play beginning narration before loading Level 1. 
            {
                storyController.BeginStory(
                    storyLines,
                    level >= SceneManager.sceneCountInBuildSettings,
                    level
                );
            }
        }
        
        else 
        {
            Debug.LogError("Level " + level + " not found in build settings.");
        }
    }
    public void ShowScoreboard()
    {
        audioSource.Play();
        scoreboardPanel.SetActive(true); 
        creditsPanel.SetActive(false);
        ScoreboardUI scoreboard = scoreboardPanel.GetComponentInChildren<ScoreboardUI>();
        scoreboard.displayHighScores(); 
    }
    public void ShowCredits()
    {
        audioSource.Play();
        scoreboardPanel.SetActive(false); 
        creditsPanel.SetActive(true);
    }
    public void close()
    {
        audioSource.Play();
        scoreboardPanel.SetActive(false); 
        creditsPanel.SetActive(false); 
    }
    
}