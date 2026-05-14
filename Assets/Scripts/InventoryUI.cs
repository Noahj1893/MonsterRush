// Ethan Le (5/14/2026):
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Script to handle the Weapons Inventory UI that displays on far left edge of player's screen.
**/
public class InventoryUI : MonoBehaviour
{
    public List<Image> slotIcons; // The icons where the weapons images will appear when collected. 
    public List<GameObject> borderHighlights; // Highlight borders (only one slot will be highlighted at a time -- the current weapon the player has equipped). 

    GameManager gameManager; 

    void Start()
    {
        gameManager = GameManager.Instance; // Grab the game state. 

        gameManager.OnWeaponChanged += UpdateUI; // Subscribe to the event that updates Weapons Inventory UI whenever the inventory changes. 

        UpdateUI(); // Run just once when the player starts the game for the first time. 
    }

    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnWeaponChanged -= UpdateUI; // Unsubscribe to clean up memory. 
        }
    } 

    void UpdateUI()
    {
        int currentIndex = gameManager.currWeaponIndex; // Get game state's current weapon. 
        var inventory = gameManager.weaponsInventory; // Get game state's current weapons inventory. 

        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (i < inventory.Count)
            {
                // Show weapon icon:
                slotIcons[i].enabled = true; // If the weapon is in player's storage, turn its picture on. 
                slotIcons[i].sprite = inventory[i].icon; // Grab the appropriate weapon image to show on Weapons Inventory UI. 

                // Highlight current weapon:
                borderHighlights[i].SetActive(i == currentIndex); // Highlight border around active weapon. 
            }
            else
            {
                // Empty slot:
                slotIcons[i].enabled = false; // Do not turn on picture. 
                borderHighlights[i].SetActive(false); // No highlight. 
            }
        }
    }
}