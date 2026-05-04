// Ethan Le (5/4/2026):
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
    public void AttemptUse(EnemyHealth enemy, PlayerDamageable player)
    {
        if (!CanUse())
        {
            return; // Do nothing if the weapon cannot be used again yet. 
        }

        Use(enemy, player); 
        nextUseTime = Time.time + cooldownUse; // Set the next use time to the current time plus the cooldown. 
    }

    protected abstract void Use(EnemyHealth enemy, PlayerDamageable player); // Abstract method to be implemented by subclasses. 
}