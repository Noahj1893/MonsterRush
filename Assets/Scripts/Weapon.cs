// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine; 

/**
 * Abstract superclass for all weapons. All weapons subclasses will inherit this superclass. 
 **/
public abstract class Weapon : MonoBehaviour
{
    public float cooldownUse = 1f; // Base cooldown is 1 second. 
    protected float nextUseTime; // Time when the weapon can be used again. 

    // Function to check if the weapon can be used again. 
    public bool CanUse()
    {
        return Time.time >= nextUseTime; 
    }

    // Function to try to use the weapon on an enemy.
    public bool AttemptUse(List<EnemyHealth> enemies, PlayerDamageable player)
    {
        if (!CanUse())
        {
            return false; // Do nothing if the weapon cannot be used again yet. 
        }

        Use(enemies, player); 
        nextUseTime = Time.time + cooldownUse; // Set the next use time to the current time plus the cooldown (current time never resets, so we take that plus the cooldown to offset the next time the player can use a weapon). 
        return true; 
    }

    protected abstract void Use(List<EnemyHealth> enemies, PlayerDamageable player); // Abstract method to be implemented by subclasses. 

    // Enum for different weapon types so we can fetch the correct animation:
    public enum WeaponAnimType
    {
        Sword, 
        FireWand,
        HealingShield,
        IceHammer
    }

    // Variable of the enum type: 
    public WeaponAnimType animType; 
}