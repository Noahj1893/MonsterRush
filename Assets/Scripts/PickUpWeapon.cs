// Ethan Le (5/5/2026):
using UnityEngine; 

/**
 * Script to manage when player picks up a new weapon upon contact.
**/
public class PickUpWeapon : MonoBehaviour
{
    [SerializeField] Weapon weaponPrefab; // Script gets attached to the GameObject for the weapon that is displayed in the level; assign the specific weapon's prefab in Unity's Inspector. 

    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController player = col.GetComponent<PlayerController>(); 

        if (player != null) // This checks to see if the entity that has touched the new weapon is the player. 
        {
            player.AddWeapon(weaponPrefab); // Add the weapon to the player's inventory if the player has touched it. 
            Destroy(gameObject); // Get rid of the weapon from the level scene after collecting it. 
        }
    }
}