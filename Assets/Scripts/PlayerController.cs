using System; 
using System.Collections.Generic; // For List<>. 
using System.Linq; // For using Reverse() to get top Keys from the Sorted Dictionary.
using System.Text; // For StringBuilder. 
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager; 
    public float speed = 5f;
    public float jumpForce = 5f;
    public float attackRange = 0.9f;
    public float attackForwardOffset = 0.45f;
    public float attackUpOffset = 0.45f;
    public int attackDamage = 1;
    public float dashSpeed = 9f; // 9x speed for when the player is dashing (C key is pressed). 
    public float dashDuration = 0.25f; // 0.25 seconds of dashing. 
    public float dashCooldown = 0.8f; // 0.8 seconds of cooldown after dashing. 
    [SerializeField] Transform firePos; // For the fireball's position and spin rotation. 

    // Player score:
    [SerializeField] ScoreUI scoreUI; // Assign ScoreUI GameObject (has text components for score display) via Unity Inspector. 

    // Weapon variables:
    Weapon currWeapon; // Player's current weapon.
    PlayerDamageable playerDamageable; // Needed for handling healing effect. 
    Dictionary<int, float> weaponCooldowns = new Dictionary<int, float>();

    [SerializeField] private LayerMask whatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D crouchDisableCollider;				// A collider that will be disabled when crouching

    bool isDashing = false; // Flag to mark when the player is dashing. 
    bool canDash = true; // Flag to mark when the player can dash again. 
    bool isCrouching = false;
    float dashTimeLeft; // Time left for the dash. 
    float dashDirection; // Direction of the dash. 


   Rigidbody2D rb;
   Animator animator;
   float moveInput;
   float verticalInput;
   bool grounded;
   float facingX;
   
   /**
   BoxCollider2D playerBoxCol; 
   Vector2 originalBoxColSize; 
   Vector2 crouchSize = new Vector2(1f, 0.5f); // Size of the player's box collider when crouching. 
   Vector2 originalOffset; 
   **/
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        gameManager = GameManager.Instance; 
        gameManager.score = 0; // Reset backend score after finishing the level. 
        facingX = Mathf.Sign(transform.localScale.x);
        if (facingX == 0f) facingX = 1f;

        playerDamageable = GetComponent<PlayerDamageable>(); // Get PlayerDamageable component. 

        currWeapon = gameManager.weaponsInventory[gameManager.currWeaponIndex]; // Default weapon is the first one in inventory (the Sword). 
    
        // If the current weapon index equipped is below min, return min. If above max, return max. 
        // Otherwise, return actual index.
        if (gameManager.weaponsInventory.Count > 0)
        {
            gameManager.currWeaponIndex = Mathf.Clamp(gameManager.currWeaponIndex, 0, gameManager.weaponsInventory.Count - 1); 
            currWeapon = gameManager.weaponsInventory[gameManager.currWeaponIndex]; // Assign new weapon based on index. 
        }
        else
        {
            currWeapon = null; 
            Debug.LogError("Inventory is EMPTY at Start()");
        }
    }

    void Update()
    {
        // If the current weapon index equipped is below min, return min. If above max, return max. 
        // Otherwise, return actual index.
        if (gameManager.weaponsInventory.Count > 0)
        {
            gameManager.currWeaponIndex = Mathf.Clamp(gameManager.currWeaponIndex, 0, gameManager.weaponsInventory.Count - 1); 

            currWeapon = gameManager.weaponsInventory[gameManager.currWeaponIndex]; // Assign new weapon based on index. 
        }
        else
        {
            currWeapon = null; 
            Debug.LogError("Inventory is EMPTY at Start()");
        }

        float scrollInventory = Input.mouseScrollDelta.y; // Either less than or greater than 0 is returned. 

        if (scrollInventory > 0f) // Scroll right in the inventory. 
        {
            SwitchWeapon(1); // Shift 1 index forward. 
        }

        else if (scrollInventory < 0f) // Scroll left in the inventory. 
        {
            SwitchWeapon(-1); // Shift 1 index backward. 
        }

        // Alternative option for the player: press keys 1 to 4 to swap to a certain weapon in their inventory.
        if (Input.GetKeyDown(KeyCode.Alpha1)) // If key 1 is pressed, 
        {
            SelectWeaponUsingIndex(0); // swap to first weapon in inventory. 
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // If key 2 is pressed, 
        {
            SelectWeaponUsingIndex(1); // swap to second weapon in inventory. 
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // If key 3 is pressed, 
        {
            SelectWeaponUsingIndex(2); // swap to third weapon in inventory. 
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) // If key 4 is pressed, 
        {
            SelectWeaponUsingIndex(3); // swap to fourth weapon in inventory. 
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
    private void FixedUpdate()
	{
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
			}
		}
	}

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        verticalInput = input.y;
    }

    public void OnJump()
    {
        if (!grounded) return;

        grounded = false;

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
        if (currWeapon == null)
        {
            Debug.LogError("currWeapon is NULL — attack cannot happen");
            return;
        }

        // Logic to define where player's weapon will hit. 
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingX * attackForwardOffset, attackUpOffset);
        //Debug.DrawLine(transform.position, attackCenter, Color.yellow, 1f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange);

        List<EnemyHealth> enemiesAttacked = new List<EnemyHealth>(); // For if a GROUP of enemies were hit at once. 

        foreach (var hit in hits)
        {
            EnemyHealth targetEnemy = hit.GetComponent<EnemyHealth>();
            GhostChaser gC = hit.GetComponent<GhostChaser>();
            if (targetEnemy != null)
            {
                enemiesAttacked.Add(targetEnemy); // Add each enemy in the hit range into the list. 
            }
            if (gC)
            {
                gC.Displace();
            }
        }

        bool attackPerformed = false;

        int weaponID = currWeapon.GetInstanceID(); 

        if (!weaponCooldowns.ContainsKey(weaponID))
        {
            weaponCooldowns[weaponID] = 0f;
        }

        if (Time.time >= weaponCooldowns[weaponID])
        {
            currWeapon.UseWeapon(enemiesAttacked, playerDamageable, firePos);
            weaponCooldowns[weaponID] = Time.time + currWeapon.cooldownUse;
            attackPerformed = true;
        }

        if (attackPerformed)
        {
            PlayWeaponAnimation(currWeapon.animType); // Play attack animation of the appropriate weapon. 
        }
    }

    // Function to switch weapons using inventory:
    void SwitchWeapon(int direction)
    {
        if (gameManager.weaponsInventory.Count == 0) // Safety check to ensure player always has at least one weapon. 
        {
            return; 
        }

        // If the current weapon index equipped is below min, return min. If above max, return max. 
        // Otherwise, return actual index.
        if (gameManager.weaponsInventory.Count > 0)
        {
            gameManager.currWeaponIndex = Mathf.Clamp(gameManager.currWeaponIndex, 0, gameManager.weaponsInventory.Count - 1); 
        }
        else
        {
            gameManager.currWeaponIndex = 0; 
        }

        int index = gameManager.currWeaponIndex; // Grab original weapon index. 

        gameManager.currWeaponIndex = (gameManager.currWeaponIndex + direction + gameManager.weaponsInventory.Count) % gameManager.weaponsInventory.Count; // Shift the index. 

        currWeapon = gameManager.weaponsInventory[gameManager.currWeaponIndex]; // Assign new weapon based on index. 

        if (index != gameManager.currWeaponIndex) // Only fire event if weapon was actually changed: 
        {
            gameManager.AlertWeaponChanged(); // Fire event to update Weapons Inventory UI. 
        }

        //Debug.Log("Current weapon slot: " + gameManager.currWeaponIndex); 
    }

    // Function for alternative way of swapping weapons (directly by index). 
    void SelectWeaponUsingIndex(int index) 
    {
        if (gameManager.weaponsInventory.Count < (index+1)) // Safety check to ensure player does not click a key number greater than their current weapons inventory capacity. 
        {
            //Debug.Log("Key pressed: " + (index+1) + " but only " + gameManager.weaponsInventory.Count); 
            return; 
        }

        // If the current weapon index equipped is below min, return min. If above max, return max. 
        // Otherwise, return actual index.
        if (gameManager.weaponsInventory.Count > 0)
        {
            gameManager.currWeaponIndex = Mathf.Clamp(gameManager.currWeaponIndex, 0, gameManager.weaponsInventory.Count - 1); 
        }
        else
        {
            gameManager.currWeaponIndex = 0; 
        }

        int oldIndex = gameManager.currWeaponIndex; // Grab original weapon index. 

        gameManager.currWeaponIndex = index; // Have the game state remember the weapon index choice. 

        currWeapon = gameManager.weaponsInventory[gameManager.currWeaponIndex]; // Assign new weapon based on index. 

        if (oldIndex != index) // Only fire event if weapon was actually changed: 
        {
            gameManager.AlertWeaponChanged(); // Fire event to update Weapons Inventory UI. 
        }

        //Debug.Log("Current weapon slot: " + gameManager.currWeaponIndex); 
    }

    // Function to pick up a new weapon:
    public void AddWeapon(Weapon newWeapon)
    {
        // Ensure there are no duplicates (in case multiple weapon instances exist in the game):
        foreach (var weapon in gameManager.weaponsInventory)
        {
            if (weapon.GetType() == newWeapon.GetType()) // If it is the same weapon (ice hammer, sword, fire wand, healing shield),
            {
                return; // then do not add it into the player's inventory. 
            }
        }

        gameManager.weaponsInventory.Add(newWeapon); // Add the new instance of the weapon into the player's inventory. 
        
        gameManager.AlertWeaponChanged(); // Fire event to update Weapons Inventory UI. 
        
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
}
