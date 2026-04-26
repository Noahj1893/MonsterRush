using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour

{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float attackCooldown = 0.45f;
    public float attackRange = 0.9f;
    public float attackForwardOffset = 0.45f;
    public int attackDamage = 1;
    public float dashSpeed = 9f; // 9x speed for when the player is dashing (C key is pressed). 
    public float dashDuration = 0.25f; // 0.25 seconds of dashing. 
    public float dashCooldown = 0.8f; // 0.8 seconds of cooldown after dashing. 
    
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
   float nextAttackTime;
   
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
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //playerBoxCol = GetComponent<BoxCollider2D>(); 
        //originalBoxColSize = playerBoxCol.size; // Store the original size of the player's box collider. 
        //originalOffset = playerBoxCol.offset; // Store the original offset of the player's box collider. 
        facingX = Mathf.Sign(transform.localScale.x);
        if (facingX == 0f) facingX = 1f;
    }

    void Update()
    {
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

        // Normal movement: 
        /**if (isCrouching) // When crouching. 
        {
            rb.linearVelocity = new Vector2(moveInput * speed * 0.2f, rb.linearVelocity.y); // Move at 20% of the normal speed when the player is crouching. 
        }

        else // When not crouching. 
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        }
        **/ 

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
        if (animator == null || Time.time < nextAttackTime || isCrouching)
            return;

        animator.SetTrigger("attack");
        PerformAttackHit();
        nextAttackTime = Time.time + attackCooldown;
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
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingX * attackForwardOffset, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if(hits[i].CompareTag("Enemy"))
            {
                EnemyHealth eH = hits[i].GetComponent<EnemyHealth>();
                if (eH != null) eH.TakeHit(attackDamage);
            }
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
}
