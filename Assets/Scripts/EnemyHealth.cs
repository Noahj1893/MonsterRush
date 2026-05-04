// Ethan Le (5/4/2026):
using UnityEngine;
using System.Collections; // For IEnumator. 

/**
 * Script to handle enemy health and status effects.
**/
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int hitsToDefeat = 1;
    [SerializeField] public EnemyType enemyType; // Get the enemy type. 

    int hitsTaken;
    bool isFrozen; // Status effect for Ice Hammer.
    bool isBurning; // Status effect for Fire Wand. 

    public void TakeHit(int damage)
    {
        if (damage <= 0 || !gameObject.activeInHierarchy)
            return;

        hitsTaken += damage;
        if (hitsTaken >= hitsToDefeat)
            gameObject.SetActive(false);
    }

    // Function to freeze the enemy for a duration (if attacked by Ice Hammer). 
    public void ApplyFreeze(float freezeDuration)
    {
        if (!isFrozen) // If enemy is not yet frozen, freeze it.
        {
            StartCoroutine(FreezeRoutine(freezeDuration));
        }
    }

    // Coroutine for freezing the enemy (timer):
    IEnumerator FreezeRoutine(float freezeDuration)
    {
        isFrozen = true; // Set enemy flag to frozen. 
        DisableMovement(); // Pause the enemy in place. 

        yield return new WaitForSeconds(freezeDuration); // Pause at this line of code for specified duration before continuing. 

        EnableMovement(); // Allow enemy to move again. 
        isFrozen = false; // Set enemy flag to unfrozen. 
    }

    // Function to burn enemy for a duration (if attacked by Fire Wand):
    public void ApplyBurn(float burnTime)
    {
        if (!isBurning) // If enemy is not yet burning, burn it. 
        {
            StartCoroutine(BurnRoutine(burnTime)); 
        }
    }

    // Coroutine for burning the enemy (timer):
    IEnumerator BurnRoutine(float burnTime)
    {
        isBurning = true; // Set the enemy flag to burning. 

        float burningTimer = 0f; // Timer to track burning. 

        int dmg = 1; // 1 burn damage per second. 

        while (burningTimer < burnTime) // While enemy is still burning, 
        {
            TakeHit(dmg); // Damage the enemy. 
            
            yield return new WaitForSeconds(1f); // Pause at this line of code for 1 second before continuing (1 second before taking another HP away from enemy). 

            burningTimer += 1f; // Burn time increments (otherwise, enemy burns indefinitely). 
        }

        isBurning = false; // Set enemy flag to unburn after burn time is up. 
    }

    // For freeze status:
    void DisableMovement()
    {
        var fallingEnemy = GetComponent<FallingEnemy>(); // Get falling enemy if it exists. 
        if (fallingEnemy != null)
        {
            fallingEnemy.enabled = false; // Freeze enemy. 
        }

        var patrolEnemy = GetComponent<PatrolEnemy>(); // Get patrol enemy if it exists. 
        if (patrolEnemy != null)
        {
            patrolEnemy.enabled = false; // Freeze enemy. 
        }

        var ghostChaser = GetComponent<GhostChaser>(); // Get ghost chaser if it exists. 
        if (ghostChaser != null)
        {
            ghostChaser.enabled = false; // Freeze enemy. 
        }
    }

    // For freeze status:
    void EnableMovement()
    {
        var fallingEnemy = GetComponent<FallingEnemy>(); // Get falling enemy if it exists. 
        if (fallingEnemy != null)
        {
            fallingEnemy.enabled = true; // Unfreeze enemy. 
        }

        var patrolEnemy = GetComponent<PatrolEnemy>(); // Get patrol enemy if it exists. 
        if (patrolEnemy != null)
        {
            patrolEnemy.enabled = true; // Unfreeze enemy. 
        }

        var ghostChaser = GetComponent<GhostChaser>(); // Get ghost chaser if it exists. 
        if (ghostChaser != null)
        {
            ghostChaser.enabled = true; // Unfreeze enemy. 
        }
    }
}
