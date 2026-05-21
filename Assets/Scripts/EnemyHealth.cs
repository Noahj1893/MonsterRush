// Ethan Le (5/4/2026):
using TMPro; 
using System.Collections; // For IEnumator. 
using UnityEngine;
using UnityEngine.UI; 

/**
 * Script to handle enemy health and status effects.
**/
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHP;
    [SerializeField] int currentHP; 
    [SerializeField] public EnemyType enemyType; // Get the enemy type. 
    [SerializeField] TextMeshProUGUI hpText; // For Enemy HP text display.
    [SerializeField] Image hpBar; // For Enemy HP bar display. 
    [SerializeField] TextMeshProUGUI statusEffectText; // For enemy's status effect ("frozen", "burning"). 
    [SerializeField] bool isBoss = false;
    [SerializeField] Animator animator;

    bool isFrozen; // Status effect for Ice Hammer.
    bool isBurning; // Status effect for Fire Wand. 
    ScoreUI scoreUI; 

    void Start()
    {
        // Retrieve Score UI component through tag:
        var obj = GameObject.FindGameObjectWithTag("Scoreboard"); 
        if (obj != null)
        {
            scoreUI = obj.GetComponent<ScoreUI>();
        }
        
        if (scoreUI == null)
        {
            Debug.Log("ScoreUI not retrieved!");
        }

        if (hpText == null)
        {
            Debug.Log("HP text not retrieved!");
        }

        if (hpBar == null)
        {
            Debug.Log("HP bar not retrieved!");
        }

        if (statusEffectText == null)
        {
            Debug.Log("Status Effect text not retrieved!"); 
        }

        currentHP = maxHP; // Enemy has full health. 
        UpdateHPUI(); // Load enemy's health visuals. 
    }

    public void TakeHit(int damage)
    {
        if (damage <= 0 || !gameObject.activeInHierarchy)
            return;

        currentHP -= damage;
        UpdateHPUI(); // Update enemy's health visuals. 
        if (currentHP <= 0)
        {
            GameManager.Instance.score += 10; 
            scoreUI.UpdateUI(GameManager.Instance.score, GameManager.Instance.deathCount);  
            if(isBoss)
            {
                GetComponent<Boss>().Die();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void UpdateHPUI()
    {
        if (hpText != null)
        {
            hpText.text = currentHP.ToString(); // Display the enemy's current HP as text. 
        }

        if (hpBar != null)
        {
            hpBar.fillAmount = (float) currentHP / maxHP; // Fill the image (health bar) based on percentage ratio between currentHP and maxHP. 
        }
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
        statusEffectText.text = "Frozen!"; 
        DisableMovement(); // Pause the enemy in place. 

        yield return new WaitForSeconds(freezeDuration); // Pause at this line of code for specified duration before continuing. 

        EnableMovement(); // Allow enemy to move again. 
        statusEffectText.text = ""; 
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

        statusEffectText.text = "Burning!"; 

        float burningTimer = 0f; // Timer to track burning. 

        int dmg = 1; // 1 burn damage per second. 

        while (burningTimer < burnTime) // While enemy is still burning, 
        {   
            yield return new WaitForSeconds(1f); // Pause at this line of code for 1 second before continuing (1 second before taking another HP away from enemy). 

            burningTimer += 1f; // Burn time increments (otherwise, enemy burns indefinitely). 

            TakeHit(dmg); // Damage the enemy. 
        }

        statusEffectText.text = ""; 

        isBurning = false; // Set enemy flag to unburn after burn time is up. 
    }

    // For freeze status:
    void DisableMovement()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        Animator enemyAnimator = null; // Initialize variable to get enemy Animator component. 

        var fallingEnemy = GetComponent<FallingEnemy>(); // Get falling enemy if it exists. 
        if (fallingEnemy != null)
        {
            enemyAnimator = fallingEnemy.GetComponent<Animator>(); 
            enemyAnimator.speed = 0;
            fallingEnemy.enabled = false; // Freeze enemy. 
        }

        var patrolEnemy = GetComponent<PatrolEnemy>(); // Get patrol enemy if it exists. 
        if (patrolEnemy != null)
        {
            enemyAnimator = patrolEnemy.GetComponent<Animator>(); 
            enemyAnimator.speed = 0;
            patrolEnemy.enabled = false; // Freeze enemy. 
        }

        var ghostChaser = GetComponent<GhostChaser>(); // Get ghost chaser if it exists. 
        if (ghostChaser != null)
        {
            enemyAnimator = ghostChaser.GetComponent<Animator>(); 
            enemyAnimator.speed = 0;
            ghostChaser.enabled = false; // Freeze enemy. 
        }

        var boss = GetComponent<Boss>(); // Get boss enemy if it exists.
        if (boss != null)
        {
            boss.isFrozen = true; // Boss's attacks are frozen too. 
            boss.animator.speed = 0; 
        }
    }

    // For freeze status:
    void EnableMovement()
    {
        Animator enemyAnimator = null; // Initialize variable to get enemy Animator component. 

        var fallingEnemy = GetComponent<FallingEnemy>(); // Get falling enemy if it exists. 
        if (fallingEnemy != null)
        {
            enemyAnimator = fallingEnemy.GetComponent<Animator>(); 
            enemyAnimator.speed = 1;
            fallingEnemy.enabled = true; // Unfreeze enemy. 
        }

        var patrolEnemy = GetComponent<PatrolEnemy>(); // Get patrol enemy if it exists. 
        if (patrolEnemy != null)
        {
            enemyAnimator = patrolEnemy.GetComponent<Animator>(); 
            enemyAnimator.speed = 1;
            patrolEnemy.enabled = true; // Unfreeze enemy. 
        }

        var ghostChaser = GetComponent<GhostChaser>(); // Get ghost chaser if it exists. 
        if (ghostChaser != null)
        {
            enemyAnimator = ghostChaser.GetComponent<Animator>(); 
            enemyAnimator.speed = 1;
            ghostChaser.enabled = true; // Unfreeze enemy. 
        }

        var boss = GetComponent<Boss>(); // Get boss enemy if it exists.
        if (boss != null)
        {
            boss.isFrozen = false; // Boss's attacks are unfrozen too. 
            boss.animator.speed = 1; 
        }
    }
}
