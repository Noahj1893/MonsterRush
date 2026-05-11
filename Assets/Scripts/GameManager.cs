// Ethan Le (5/11/2026):
using System; 
using System.Collections.Generic; // For List<>. 
using System.Linq; // For using Reverse() to get top Keys from the Sorted Dictionary.
using System.Text; // For StringBuilder. 
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score;

    public SortedDictionary<int, SortedSet<int>> playerData = new SortedDictionary<int, SortedSet<int>>(); // For player's top 5 scores for each level in the game.  

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}