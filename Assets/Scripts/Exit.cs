using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    bool hasExited = false; 
    bool canExit = false;
    SpriteRenderer sr;
    Sprite openDoor;
    public GameObject enemiesHolder;
    public GameObject storyUI; // Assign GameObject for Story UI via Unity Inspector. 
    [SerializeField] private string[] storyLines;
    [SerializeField] private StoryController storyController;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if(enemiesHolder == null)
        {
            enemiesHolder = GameObject.Find("Enemies");
        }
    }
    void Update()
    {
        if (canExit) return;
        int enemies = enemiesHolder.GetComponentsInChildren<EnemyHealth>().Length;
        Debug.Log(enemies);
        if (enemies == 0) {
            canExit = true;
            sr.color = new Color(1, 1, 1, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasExited && canExit) 
        {
            hasExited = true; 
            ExitLevel();
        }
    }

    void ExitLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex;

        // Write brand new data for the level if player had not completed the level prior:
        if (!GameManager.Instance.playerData.ContainsKey(levelNumber))
        {
            GameManager.Instance.playerData[levelNumber] = new SortedSet<int>(); // Create new Sorted Set for the level (holds all scores for that level). 
        }

        // Use level number and set player's new score for that level into the Sorted Dictionary player's data.
        GameManager.Instance.playerData[levelNumber].Add(GameManager.Instance.score); 

        // Exit the level
        Debug.Log(levelNumber);
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if(levelNumber + 1 >= SceneManager.sceneCountInBuildSettings) {
            storyController.BeginStory(
                storyLines,
                levelNumber + 1 >= SceneManager.sceneCountInBuildSettings,
                1
            );
            Debug.Log("Last Level Reached!"); 
        }
        else
        {
            storyController.BeginStory(
                storyLines,
                levelNumber + 1 >= SceneManager.sceneCountInBuildSettings,
                levelNumber + 1
            );
        }
        
    }
}
