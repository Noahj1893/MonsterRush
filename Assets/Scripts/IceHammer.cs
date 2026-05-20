// Ethan Le (5/4/2026):
using System.Collections.Generic; // For List<>. 
using UnityEngine; 

/** 
 * Script for the Ice Hammer weapon. Inherits from Weapon.cs superclass. 
**/
[CreateAssetMenu(menuName = "Weapons/Ice Hammer")]
public class IceHammer : Weapon 
{
    [SerializeField] float freezeDuration = 1f;

    private void OnEnable()
    {
        cooldownUse = 2f; // Cooldown for attack is 2 seconds. 
        animType = WeaponAnimType.IceHammer; // Animation type is IceHammer. 
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(List<EnemyHealth> enemies, PlayerDamageable player, Transform firePos)
    {
        foreach (var enemy in enemies) // Ice Hammer does AREA attack (can hit multiple targets). 
        {
            int dmg = 1; // Base damage is 1. 

            // Do double damage to enemies weak to ice: 
            if (enemy.enemyType == EnemyType.Fire)
            {
                dmg *= 2;
            }

            enemy.TakeHit(dmg); // Apply damage to the enemy (defined in EnemyHealth.cs class). 
            enemy.ApplyFreeze(freezeDuration); // Apply freeze status to the enemy (defined in EnemyHealth.cs class). 
        }
    }

}