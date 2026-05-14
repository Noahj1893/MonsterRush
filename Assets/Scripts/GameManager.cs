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

    // Weapon variables:
    [SerializeField] Weapon[] startingWeapon; // Temporary array to always store player's starting weapon (the Sword) in the backend.
    public List<Weapon> weaponsInventory = new List<Weapon>(); // Actual player's inventory (gets assigned the stuff in the temp inventory).  
    public int currWeaponIndex = 0; // Marks the player's current weapon based on slot position in the Inventory. 

    // Event that triggers when the Weapons inventory has changed to alert the Weapons Inventory UI to update:
    public event Action OnWeaponChanged; 

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

    void Start() 
    {
        // Create a brand new inventory for the player upon starting the game:
        weaponsInventory.Clear(); // Always a fresh new inventory upon start. 

        // Add any starting weapons stored in the backend into the new inventory:
        foreach (var weapon in startingWeapon)
        {
            Weapon instance = Instantiate(weapon, transform); // Create an instance of the starting weapons. 
            weaponsInventory.Add(instance); // Add it to the player's inventory. 
        }
    }

    public void AlertWeaponChanged()
    {
        OnWeaponChanged?.Invoke(); // Other scripts cannot trigger the event, they can only subscribe to it. 
        // So the GameManager has to be the one to actually trigger the event. 
    }
}