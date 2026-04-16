
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

   Rigidbody2D rb;
   Animator animator;
   float moveInput;
   bool grounded;
   float facingX;
   float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        facingX = Mathf.Sign(transform.localScale.x);
        if (facingX == 0f) facingX = 1f;
    }

    void Update()
    {
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

    public void OnAttack()
    {
        if (animator == null || Time.time < nextAttackTime)
            return;

        animator.SetTrigger("attack");
        PerformAttackHit();
        nextAttackTime = Time.time + attackCooldown;
    }

    void PerformAttackHit()
    {
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingX * attackForwardOffset, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange);
        for (int i = 0; i < hits.Length; i++)
        {
            GhostHealth ghostHealth = hits[i].GetComponent<GhostHealth>();
            if (ghostHealth != null)
                ghostHealth.TakeHit(attackDamage);

            AlienHealth alienHealth = hits[i].GetComponent<AlienHealth>();
            if (alienHealth != null)
                alienHealth.TakeHit(attackDamage);
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
