// Ethan Le (5/4/2026):
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
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(EnemyHealth enemy, PlayerDamageable player)
    {
        // Create a fireball on the screen that gets launched. 
        Instantiate(fireballPrefab, firePos.position, firePos.rotation); 
    }
}