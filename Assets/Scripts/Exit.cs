using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{
    bool canLeave;
    bool hasExited = false; 

    void Start()
    {
        canLeave = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) canLeave = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) canLeave = false;
    }
    void Update()
    {
        if(canLeave && !hasExited) {
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
            Debug.Log("Last Level Reached!"); 
            SceneManager.LoadSceneAsync("Title"); ///load main menu
        }
        else
        {
            SceneManager.LoadSceneAsync(levelNumber + 1);
        }
        
    }
}
