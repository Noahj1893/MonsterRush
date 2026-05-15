// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine; 

/**
 * Abstract superclass for all weapons. All weapons subclasses will inherit this superclass. 
 **/
public abstract class Weapon : ScriptableObject
{
    public float cooldownUse = 1f; // Base cooldown is 1 second. 
    public Sprite icon; // Assign Weapon icon to each Weapon prefab in Unity Inspector. 

    protected abstract void Use(List<EnemyHealth> enemies, PlayerDamageable player, Transform firePos); // Abstract method to be implemented by subclasses. 

    public void UseWeapon(List<EnemyHealth> enemies, PlayerDamageable player, Transform firePos)
    {
        Use(enemies, player, firePos);
    }

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