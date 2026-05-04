// Ethan Le (5/4/2026):
using UnityEngine; 

/**
 * Script for Healing Shield weapon. Inherits from Weapon.cs superclass. 
**/
public class HealingShield : Weapon
{
    void Awake()
    {
        cooldownUse = 3f; // Cooldown for healing is 3 seconds. 
    }

    protected override void Use(EnemyHealth enemy, PlayerDamageable player)
    {
        int restoreHP = 1; 

        if (player != null) // Ensure the player still exists. 
        {
            player.HealPlayer(restoreHP); // Call PlayerDamageable.cs script to heal the player. 
        }
    }
}