using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{
    bool canLeave;
    PlayerController player; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); 
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
        if(canLeave) {
            ExitLevel();
        }
    }


    void ExitLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex;

        player.SetData(levelNumber, player.GetScore()); // Use level number and set player's new score for that level into the Sorted Dictionary player's data.
         
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
