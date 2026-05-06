// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine; 

/** 
 * Script for the Fire Wand weapon. Inherits from Weapon.cs superclass. 
**/
public class FireWand : Weapon
{
    [SerializeField] GameObject fireballPrefab; // Assign prefab for fireball animation in Unity Inspector. 
    [SerializeField] Transform firePos; // For the fireball's position and spin rotation. 

    void Awake()
    {
        cooldownUse = 3f; // Cooldown for attack is 3 seconds. 
        animType = WeaponAnimType.FireWand; // Animation type is FireWand. 
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(List<EnemyHealth> enemies, PlayerDamageable player)
    {
        // Create a fireball on the screen that gets launched. 
        GameObject fireball = Instantiate(fireballPrefab, firePos.position, firePos.rotation); 

        // Set the direction it should move accordingly (+1 is facing right, -1 is facing left):
        float playerFacing = Mathf.Sign(player.transform.localScale.x); // Direction is based on where the player is facing. 

        fireball.GetComponent<Fireball>().Init(playerFacing); // Update direction of fireball's movement accordingly. 
    }

    // Function to create spawn position of the fireballs:
    public void SetFirePos(Transform position)
    {
        firePos = position; 
    }
}