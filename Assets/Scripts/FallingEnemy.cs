using UnityEngine;

public class FallingEnemy : MonoBehaviour
{
    [SerializeField] private Vector2 fallPoint;
    [SerializeField] private float minY;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private Animator animator;

    bool isGrounded = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        animator.SetBool("isGrounded", isGrounded);

        float yVel = rb.linearVelocity.y;
        if(!isGrounded) rb.linearVelocity = new Vector2(0, yVel);
        else rb.linearVelocity = new Vector2(-speed, yVel);
    }
    void FixedUpdate()
    {
        isGrounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckDistance, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
			}
		}

        if (rb.position.y <= minY)
        {
            animator.Play("Falling", 0, 0f);
            rb.position = fallPoint;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
