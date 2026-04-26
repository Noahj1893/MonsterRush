using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] PlayerDamageable player;
    [SerializeField] Image[] HPDisplay; // Array of images that display full or empty health. 

    [SerializeField] Sprite fullHP; // Assign sprite for a piece of health.
    [SerializeField] Sprite emptyHP; // Assign sprite for empty piece of health. 

    void OnEnable() // Subscribe to event that fires when player's health has changed. 
    {
        if (player != null)
        {
            player.OnHealthChanged += UpdateUI;
        }
    }

    void OnDisable() // Unsubscribe to event that fires when player's health has changed (unsubscribe to prevent memory leaks). 
    {
        player.OnHealthChanged -= UpdateUI;
    }

    void Start()
    {
        UpdateUI(player.GetCurrentHealth(), player.GetMaxHealth()); // When starting the game, show HP display (player has full health). 
    }

    void UpdateUI(int currentHP, int maxHP)
    {
        for (int i = 0; i < maxHP; i++) // Loop through entire health bar display. 
        {
            if (i < currentHP)
            {
                HPDisplay[i].sprite = fullHP; // Show piece of health if not all currentHP is currently displayed.
            }

            else
            {
                HPDisplay[i].sprite = emptyHP; // Show empty piece of health for the remaining HP slots. 
            }

            HPDisplay[i].enabled = i < maxHP; // Turn on the HP display. 
        }
    }
}