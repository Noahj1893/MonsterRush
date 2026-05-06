// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine; 

/**
 * Script for Healing Shield weapon. Inherits from Weapon.cs superclass. 
**/
public class HealingShield : Weapon
{
    void Awake()
    {
        cooldownUse = 3f; // Cooldown for healing is 3 seconds. 
        animType = WeaponAnimType.HealingShield; // Animation type is Healing Shield. 
    }

    protected override void Use(List<EnemyHealth> enemies, PlayerDamageable player)
    {
        int restoreHP = 1; 

        if (player != null) // Ensure the player still exists. 
        {
            player.HealPlayer(restoreHP); // Call PlayerDamageable.cs script to heal the player. 
        }
    }
}