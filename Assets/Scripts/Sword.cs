// Ethan Le (5/4/2026):
using UnityEngine;

/**
 * Script for the Sword weapon. Inherits from Weapon.cs superclass. 
**/
public class Sword : Weapon
{
    void Awake()
    {
        cooldownUse = 0.45f; // Cooldown for attack is 0.45 seconds.
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(EnemyHealth enemy, PlayerDamageable player)
    {
        int dmg = 1; // Damage is 1 for the Sword, regardless of enemy type. 

        if (enemy != null)
        {
            enemy.TakeHit(dmg); 
        }
    }
}