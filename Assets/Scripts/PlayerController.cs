using System; 
using System.Collections.Generic; // For List<>. 
using System.Linq; // For using Reverse() to get top Keys from the Sorted Dictionary.
using System.Text; // For StringBuilder. 
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float attackRange = 0.9f;
    public float attackForwardOffset = 0.45f;
    public int attackDamage = 1;
    public float dashSpeed = 9f; // 9x speed for when the player is dashing (C key is pressed). 
    public float dashDuration = 0.25f; // 0.25 seconds of dashing. 
    public float dashCooldown = 0.8f; // 0.8 seconds of cooldown after dashing. 
    [SerializeField] Transform firePos; // For the fireball's position and spin rotation. 

    // Player score:
    private int score = 0; 
    SortedDictionary<int, SortedSet<int>> playerData = new SortedDictionary<int, SortedSet<int>>(); // For player's top 5 scores for each level in the game.  
    [SerializeField] ScoreUI scoreUI; // Assign ScoreUI GameObject (has text components for score display) via Unity Inspector. 

    // Weapon variables:
    [SerializeField] Weapon[] startingWeapon; // Temporary array to always store player's starting weapon (the Sword) in the backend.
    List<Weapon> weaponsInventory = new List<Weapon>(); // Actual player's inventory (gets assigned the stuff in the temp inventory).  
    int currWeaponIndex = 0; // Marks the player's current weapon based on slot position in the Inventory. 
    Weapon currWeapon; // Player's current weapon.
    PlayerDamageable playerDamageable; // Needed for handling healing effect. 

    bool isDashing = false; // Flag to mark when the player is dashing. 
    bool canDash = true; // Flag to mark when the player can dash again. 
    float dashTimeLeft; // Time left for the dash. 
    float dashDirection; // Direction of the dash. 

    bool isCrouching = false; // Flag to mark when the player is crouching. 

   Rigidbody2D rb;
   Animator animator;
   float moveInput;
   bool grounded;
   float facingX;
   
   /**
   BoxCollider2D playerBoxCol; 
   Vector2 originalBoxColSize; 
   Vector2 crouchSize = new Vector2(1f, 0.5f); // Size of the player's box collider when crouching. 
   Vector2 originalOffset; 
   **/

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.enabled = false; // Temporarily disabled. 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //playerBoxCol = GetComponent<BoxCollider2D>(); 
        //originalBoxColSize = playerBoxCol.size; // Store the original size of the player's box collider. 
        //originalOffset = playerBoxCol.offset; // Store the original offset of the player's box collider. 
        facingX = Mathf.Sign(transform.localScale.x);
        if (facingX == 0f) facingX = 1f;

        playerDamageable = GetComponent<PlayerDamageable>(); // Get PlayerDamageable component. 

        // Create a brand new inventory for the player upon starting the game:
        weaponsInventory.Clear(); // Always a fresh new inventory upon start. 

        // Add any starting weapons stored in the backend into the new inventory:
        foreach (var weapon in startingWeapon)
        {
            Weapon instance = Instantiate(weapon, transform); // Create an instance of the starting weapons. 
            weaponsInventory.Add(instance); // Add it to the player's inventory. 
        }

        currWeaponIndex = 0; 
        currWeapon = weaponsInventory[currWeaponIndex]; // Default weapon is the first one in inventory (the Sword). 
    }

    void Update()
    {
        float scrollInventory = Input.mouseScrollDelta.y; // Either less than or greater than 0 is returned. 

        if (scrollInventory > 0f) // Scroll right in the inventory. 
        {
            SwitchWeapon(1); // Shift 1 index forward. 
        }

        else if (scrollInventory < 0f) // Scroll left in the inventory. 
        {
            SwitchWeapon(-1); // Shift 1 index backward. 
        }

        // Dashing movement:
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y); // Dash with 9x speed in the direction the player is facing, but keep the vertical velocity (accounts for jumping).  

            dashTimeLeft -= Time.deltaTime; // Decrease the remaining time of the dash. 

            if (dashTimeLeft <= 0f)
            {
                isDashing = false; // Once dash time is up, player stops dashing. 
                Invoke(nameof(ResetDashCooldown), dashCooldown); // Reset the dash cooldown after 0.8 seconds. 
            }

            return; // Do not continue with normal movement after the dash is over. 
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (moveInput > 0f)
            facingX = 1f;
        else if (moveInput < 0f)
            facingX = -1f;

        Vector3 s = transform.localScale;
        float ax = Mathf.Abs(s.x);
        transform.localScale = new Vector3(facingX * ax, s.y, s.z);

        if (animator != null)
            animator.SetFloat("speed", Mathf.Abs(moveInput));
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump()
    {
        if (!grounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    /**
     * Function for when player is crouching:
    **/
    /**
    public void OnCrouch(InputAction.CallbackContext context) // Unity searches for a function called OnCrouch() when the S key is pressed (ties to "Crouch" action in InputSystem_Actions.inputactions). 
    {
        if (animator == null) 
        {
            return; // Do nothing if the animator is not found. 
        }

        if (context.started) // When S is held down. 
        {
            isCrouching = true; // Set the isCrouching flag to true. 
        }
        
        else if (context.canceled) // When S is released. 
        {
            isCrouching = false; // Set the isCrouching flag to false. 
        }

        animator.SetBool("crouching", isCrouching); // Set animator's "crouching" parameter to the value (true or false) of the isCrouching flag. 
    
        if (isCrouching)
        {
            playerBoxCol.size = crouchSize; // Set the size of the player's box collider to the set crouch size. 
            playerBoxCol.offset = new Vector2(originalOffset.x, originalOffset.y - 0.25f); // Set offset when crouching. 
        }

        else
        {
            playerBoxCol.size = originalBoxColSize; // Reset player box collider when crouch button (S) is released. 
            playerBoxCol.offset = originalOffset; // Reset offset when crouch button is released. 
        }
    }
    **/

    public void OnAttack()
    {
        if (animator == null || isCrouching)
            return;

        PerformAttackHit();
    }

    /**
     * Function for when player is TRYING to dash. 
    **/
    public void OnDash() // Unity searches for a function called OnDash() when the C key is pressed (ties to "Dash" action in InputSystem_Actions.inputactions). . 
    {
        if (!canDash || isDashing || isCrouching) // Do nothing if the player cannot dash or is currently dashing or is crouching. 
        {
            return;
        }

        BeginDash(); // Otherwise, start the dash. 
    }

    /** 
     * Function for when the player begins a dash. 
    **/
    void BeginDash()
    {
        // Player is now dashing and cannot trigger dash again. 
        isDashing = true; 
        canDash = false; 

        dashTimeLeft = dashDuration; // Dash time of 0.25 seconds is set. 

        // Lock the direction at the moment of the dash:
        dashDirection = (moveInput != 0f) ? Mathf.Sign(moveInput) : facingX; // If player is moving, set dash direction to the direction of the movement input. Otherwise, use the direction the player is currently facing as the dash direction. 
    }

    /** 
     * Function to be called after dash cooldown of 0.8 seconds is over.
    **/
    void ResetDashCooldown()
    {
        canDash = true; // Player can now dash again. 
    }

    void PerformAttackHit()
    {
        // Logic to define where player's weapon will hit. 
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingX * attackForwardOffset, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange);

        List<EnemyHealth> enemiesAttacked = new List<EnemyHealth>(); // For if a GROUP of enemies were hit at once. 

        foreach (var hit in hits)
        {
            EnemyHealth targetEnemy = hit.GetComponent<EnemyHealth>();
            if (targetEnemy != null)
            {
                enemiesAttacked.Add(targetEnemy); // Add each enemy in the hit range into the list. 
            }
        }

        bool attackPerformed = currWeapon.AttemptUse(enemiesAttacked, playerDamageable); // Based on current weapon, try to use it (gets true or false). 

        if (attackPerformed)
        {
            PlayWeaponAnimation(currWeapon.animType); // Play attack animation of the appropriate weapon. 
        }
    }

    // Function to switch weapons using inventory:
    void SwitchWeapon(int direction)
    {
        if (weaponsInventory.Count == 0) // Safety check to ensure player always has at least one weapon. 
        {
            return; 
        }

        currWeaponIndex = (currWeaponIndex + direction + weaponsInventory.Count) % weaponsInventory.Count; // Shift the index. 

        currWeapon = weaponsInventory[currWeaponIndex]; // Assign new weapon based on index. 

        Debug.Log("Current weapon slot: " + currWeaponIndex); 
    }

    // Function to pick up a new weapon:
    public void AddWeapon(Weapon newWeapon)
    {
        // Ensure there are no duplicates (in case multiple weapon instances exist in the game):
        foreach (var weapon in weaponsInventory)
        {
            if (weapon.GetType() == newWeapon.GetType()) // If it is the same weapon (ice hammer, sword, fire wand, healing shield),
            {
                return; // then do not add it into the player's inventory. 
            }
        }

        // Create the instance of the weapon:
        Weapon weaponInst = Instantiate(newWeapon, transform); 

        // Check if the weapon is a FireWand so we can assign the FirePoint automatically:
        FireWand fireWand = weaponInst as FireWand; 

        if (fireWand != null)
        {
            fireWand.SetFirePos(firePos); // If the weapon picked up is a FireWand, initialize spawn position of the fireballs. 
        }

        weaponsInventory.Add(weaponInst); // Add the new instance of the weapon into the player's inventory. 
        Debug.Log("Got new weapon!"); // Debug log. 
    }

    // Function to play the correct animation depending on the weapon the player has currently equipped:
    void PlayWeaponAnimation(Weapon.WeaponAnimType weaponType)
    {
        if (animator == null)
        {
            return; // Safety check to ensure animator component exists. 
        }

        // Switch statement to determine weapon animation accordingly: 
        switch (weaponType)
        {
            case Weapon.WeaponAnimType.Sword:
                animator.SetTrigger("sword_attack");
                break; 
            case Weapon.WeaponAnimType.FireWand:
                animator.SetTrigger("firewand_attack");
                break;
            case Weapon.WeaponAnimType.IceHammer:
                animator.SetTrigger("icehammer_attack");
                break;
            case Weapon.WeaponAnimType.HealingShield:
                animator.SetTrigger("healingshield_attack");
                break; 
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            grounded = true;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            grounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            grounded = false;
    }

    // Function to retrieve player's score: 
    public int GetScore()
    {
        return score; 
    }

    // Function to update player's score: 
    public void SetScore(int updatedScore)
    {
        score = updatedScore; 
        scoreUI.UpdateUI(score); // Update Score UI. 
    }

    // Function to reset score after game completion: 
    public void ResetScore()
    {
        score = 0;
    }

    // Function to retrieve Sorted Dictionary of player's score data:
    public SortedDictionary<int, SortedSet<int>> GetData()
    {
        return playerData;
    }

    // Function to update player's score data:
    public void SetData(int level, int levelScore)
    {
        // Write brand new data for the level if player had not completed the level prior:
        if (!playerData.ContainsKey(level))
        {
            SortedSet<int> levelData = new SortedSet<int>(); // Create new Sorted Set for the level (holds all scores for that level). 

            levelData.Add(levelScore); // Add the new score as the first score to the level's Sorted Set. 

            playerData.Add(level, levelData); // Add the level number and its corresponding Sorted Set into the Sorted Dictionary. 
        }

        // Otherwise, modify the existing data for the level if the player has already completed it before:
        else 
        {
            playerData.TryGetValue(level, out SortedSet<int> levelData); // Get the existing Sorted Set of scores for the level. 

            levelData.Add(levelScore); // Add the new score to the level's Sorted Set (sorted automatically).

            playerData[level] = levelData; // dict[key] = value; 
        }
    }
}
