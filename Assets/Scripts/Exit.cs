using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{

    void OnTriggerStay(Collider other)
    {
        Debug.Log("on door");
        if(Input.GetKeyDown(KeyCode.E))
        {
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
