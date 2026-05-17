using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; 

public class Exit : MonoBehaviour
{
    //bool hasExited = false; 
    bool canExit = false;
    SpriteRenderer sr;
    public Sprite openDoor;
    public GameObject enemiesHolder;
    public GameObject storyUI; // Assign GameObject for Story UI via Unity Inspector. 
    [SerializeField] private string[] storyLines;
    [SerializeField] private StoryController storyController;
    [SerializeField] private TextMeshProUGUI enemiesLeftText;  

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
        enemiesLeftText.text = "Enemies Left: " + enemies;
        if (enemies == 0) {
            canExit = true;
            sr.sprite = openDoor;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player") && !hasExited) 
        if (collision.CompareTag("Player")) 
        {
            if(canExit)
            {
                //hasExited = true; 
                ExitLevel();
            }
            else
            {
                enemiesLeftText.gameObject.SetActive(true);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            enemiesLeftText.gameObject.SetActive(false);
        }
    }

    void ExitLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex;

        // Write brand new data for the level if player had not completed the level prior:
        if (!GameManager.Instance.playerData.ContainsKey(levelNumber))
        {
            GameManager.Instance.playerData[levelNumber] = new SortedDictionary<int, int>(); // Create new Sorted Dictionary for the level (Key=Score, Value=Death Count). 
        }

        // Use level number and set player's new score and death count for that level into the Sorted Dictionary player's data.
        GameManager.Instance.playerData[levelNumber][GameManager.Instance.score] = GameManager.Instance.deathCount; 

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
