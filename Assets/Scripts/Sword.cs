// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine;

/**
 * Script for the Sword weapon. Inherits from Weapon.cs superclass. 
**/
[CreateAssetMenu(menuName = "Weapons/Sword")]
public class Sword : Weapon
{
    private void OnEnable()
    {
        cooldownUse = 0.45f; // Cooldown for attack is 0.45 seconds.
        animType = WeaponAnimType.Sword; // Animation type is Sword. 
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(List<EnemyHealth> enemies, PlayerDamageable player, Transform firePos)
    {
        int dmg = 1; // Damage is 1 for the Sword, regardless of enemy type. 

        if (enemies.Count > 0)
        {
            enemies[0].TakeHit(dmg); // Sword attacks only one target. 
        }
    }
}