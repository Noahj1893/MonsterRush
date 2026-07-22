// Ethan Le (5/14/2026):
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
using TMPro;

/** 
 * Script to control any story sequence:
**/ 
public class StoryController : MonoBehaviour
{
    public GameObject storyUI; // Assign GameObject for Story UI via Unity Inspector. 
    public TextMeshProUGUI storyText; // Displays the current dialogue.
    public Image storyImage; // Displays the current character speaking. 
    public Button skipButton; // For skipping current story sequence if player does not want to read. 
    [SerializeField] private MonoBehaviour playerController; // Works with both PlayerController or NewPlayerController. 
    [SerializeField] AudioSource hoverSound; // A little "ding" sound whenever dialogue continues (same sound as the menu scene).

    [TextArea(3, 5)]
    public string[] storyLines; // Array -- Fill in Unity Inspector with the narration.
    public Sprite[] storyArt; // Array -- Fill in Unity Inspector with the character art. 

    bool isStoryActive = false; // To prevent bugs with narration during gameplay. 
    private int currentIndex = 0;
    public bool endOfGame; 
    public int nextLevel; 

    void Start()
    {
        if (skipButton != null) // Ensure the Button component is assigned in Unity Inspector. 
        {
            skipButton.onClick.RemoveAllListeners(); // Ensure fresh reset of the Skip button. 
            skipButton.onClick.AddListener(SkipIntro); // Add listener to skip narration when button is pressed. 
        }
    }

    public void BeginStory(string[] lines, Sprite[] art, bool isEndGame, int next)
    {
        if (lines == null || lines.Length == 0)
        {
            //Debug.Log("StoryController: No story lines given!");
            return;
        }

        storyLines = lines;
        storyArt = art; 
        endOfGame = isEndGame;
        nextLevel = next;

        currentIndex = 0;
        isStoryActive = true;

        storyUI.SetActive(true);

        storyText.text = storyLines[currentIndex];
        storyImage.sprite = storyArt[currentIndex];

        if (playerController != null)
        {
            playerController.enabled = false; // Freeze player from any accidental movement during narration. 
        }

        ShowCurrentLine(); // Game starts at index 0 in the sequence of narration lines. 
    }

    void Update()
    {
        if (!isStoryActive)
        {
            return; // If we are not at narration, do nothing. 
        }

        // Detect X key:
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            NextLine(); // Move onto the next line of narration. 
        }
    }

    void ShowCurrentLine()
    {
        if (storyLines.Length == 0)
        {
            return; 
        }
        if (storyArt.Length == 0)
        {
            return; 
        }

        storyText.text = storyLines[currentIndex]; // Show current narration line. 
        storyImage.sprite = storyArt[currentIndex]; // Show current character art. 
    }

    void NextLine()
    {
        currentIndex++; // Increment the index so we can move on to the next narration line in the sequence. 

        hoverSound.Play(); // Play the "ding" sound whenever next line of dialogue appears. 

        if ((currentIndex < storyLines.Length) && (currentIndex < storyArt.Length)) // Show next line and art in narration if not at the end. 
        {
            ShowCurrentLine();
        }
        else
        {
            // Otherwise, start/continue the game if the narration is done:  
            currentIndex = 0;
            storyUI.SetActive(false); 
            isStoryActive = false; // Ensure next narration does not play in the backend by accident. 

            if (playerController != null)
            {
                playerController.enabled = true; // Unfreeze player after narration. 
            }

            if (endOfGame == true)
            {
                SceneManager.LoadSceneAsync("Title"); // Load main menu if end of game. 
            }
            else
            {
                SceneManager.LoadSceneAsync(nextLevel); // Next level. 
            }
        }
    }

    void SkipIntro()
    {
        // Start/continue the game if narration is skipped:
        currentIndex = 0;
        storyUI.SetActive(false); 
        isStoryActive = false; // Ensure next narration does not play in the backend by accident. 

        if (playerController != null)
        {
            playerController.enabled = true; // Unfreeze player after narration. 
        }

        if (endOfGame == true)
        {
            SceneManager.LoadSceneAsync("Title"); // Load main menu if end of game. 
        }
        else
        {
            SceneManager.LoadSceneAsync(nextLevel); // Next level. 
        }
    }
}