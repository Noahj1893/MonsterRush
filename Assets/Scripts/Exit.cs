using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{
    bool canLeave;

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
        if(canLeave) {
            ExitLevel();
        }
    }


    void ExitLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex;
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
