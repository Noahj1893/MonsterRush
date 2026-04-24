using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{
    bool canLeave = false;

   void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        canLeave = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        canLeave = false;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canLeave) {
            ExitLevel();
        }
    }


    void ExitLevel()
    {
        Debug.Log("Level Complete!");
        int levelNumber = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        // Exit the level
        if(levelNumber + 1 >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings) {
            Debug.Log("Last Level Reached!"); 
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0); ///load main menu
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelNumber + 1);
    }
}
