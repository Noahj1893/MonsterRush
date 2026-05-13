using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic; // For List<>. 

public class NewPlayerController : MonoBehaviour
{
    Animator animator;
    GameManager gameManager;
    PlayerDamageable playerDamageable; // Needed for handling healing effect. 
    [SerializeField] Weapon[] startingWeapon; // Temporary array to always store player's starting weapon (the Sword) in the backend.
    List<Weapon> weaponsInventory = new List<Weapon>(); // Actual player's inventory (gets assigned the stuff in the temp inventory).  
    int currWeaponIndex = 0; // Marks the player's current weapon based on slot position in the Inventory. 
    Weapon currWeapon; // Player's current weapon.
    public float attackRange = 0.9f;
    public float attackForwardOffset = 0.45f;
	[SerializeField] private float m_JumpForce = 5f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = true;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

    [SerializeField] Transform firePos; // For the fireball's position and spin rotation.     public float dashSpeed = 9f; // 9x speed for when the player is dashing (C key is pressed). 
    public float dashDuration = 0.25f; // 0.25 seconds of dashing. 
    public float dashCooldown = 0.8f; // 0.8 seconds of cooldown after dashing. 

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private bool crouch;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool isDashing;
    private bool canDash = false;
    float dashTimeLeft; // Time left for the dash. 
    bool dashRight; // Direction of the dash. 
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerDamageable = GetComponent<PlayerDamageable>(); // Get PlayerDamageable component. 
        animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}
    void Start()
    {
        gameManager = GameManager.Instance; 
        gameManager.score = 0; // Reset backend score after finishing the level. 
        weaponsInventory.Clear(); // Always a fresh new inventory upon start. 
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
            dashTimeLeft -= Time.deltaTime; // Decrease the remaining time of the dash. 

            if (dashTimeLeft <= 0f)
            {
                isDashing = false; // Once dash time is up, player stops dashing. 
                Invoke(nameof(ResetDashCooldown), dashCooldown); // Reset the dash cooldown after 0.8 seconds. 
            }
        }

        if (animator != null)
            animator.SetFloat("speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
    }
	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void OnAttack()
    {
        if (animator == null || crouch)
            return;

        PerformAttackHit();
    }
    public void OnDash() // Unity searches for a function called OnDash() when the C key is pressed (ties to "Dash" action in InputSystem_Actions.inputactions). . 
    {
        if (!canDash || isDashing || crouch) // Do nothing if the player cannot dash or is currently dashing or is crouching. 
        {
            return;
        }

        BeginDash(); // Otherwise, start the dash. 
    }
    void BeginDash()
    {
        // Player is now dashing and cannot trigger dash again. 
        isDashing = true; 

        dashTimeLeft = dashDuration; // Dash time of 0.25 seconds is set. 

        // Lock the direction at the moment of the dash:
        dashRight = m_FacingRight; 
    }
    void ResetDashCooldown()
    {
        canDash = true; // Player can now dash again. 
    }
    void PerformAttackHit()
    {
        // Logic to define where player's weapon will hit. 
        float right = m_FacingRight ? 1 : -1;
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(right * attackForwardOffset, 0f);
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
}