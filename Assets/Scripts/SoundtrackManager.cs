// Ethan Le (5/17/2026):
using UnityEngine;

/** 
 * Script to control soundtrack persistence.
**/
public class SoundtrackManager : MonoBehaviour 
{
    private static SoundtrackManager instance; // One singular instance for soundtrack; do not restart upon every scene reload (like when player dies or returns to title). 

    void Awake()
    {
        if (instance == null) // Create a new static instance if game was loaded up for the first time.
        {
            instance = this; 
            DontDestroyOnLoad(gameObject); // Do not restart the music between scene reloads. 
        }

        else // If an instance already exists, do not create a duplicate (AKA, destroy the new one, and keep the old). 
        {
            Destroy(gameObject); 
        }
    }
}