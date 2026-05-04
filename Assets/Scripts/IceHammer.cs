// Ethan Le (5/4/2026):
using UnityEngine; 

/** 
 * Script for the Ice Hammer weapon. Inherits from Weapon.cs superclass. 
**/
public class IceHammer : Weapon 
{
    [SerializeField] float freezeDuration = 2f;

    void Awake()
    {
        cooldownUse = 1.5f; // Cooldown for attack is 1.5 seconds. 
    }

    // Override abstract Use method from Weapon.cs superclass:
    protected override void Use(EnemyHealth enemy, PlayerDamageable player)
    {
        if (enemy == null)
        {
            return; // Do nothing if the enemy is null. 
        }

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