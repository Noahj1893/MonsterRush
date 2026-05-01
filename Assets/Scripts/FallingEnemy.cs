using UnityEngine;

public class FallingEnemy : MonoBehaviour
{
    [SerializeField] private Transform fallPoint;
    [SerializeField] private float minY;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private Animator animator;

    bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        animator.SetBool("isGrounded", isGrounded);

        float yVel = rb.linearVelocity.y;

        if (isGrounded && yVel > -1f)
            yVel = -1f;

        rb.linearVelocity = new Vector2(-speed, yVel);
    }
    void FixedUpdate()
    {
        if (rb.position.y <= minY)
        {
            rb.position = fallPoint.position;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
