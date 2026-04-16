using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostChaser : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    [SerializeField] float chaseSpeed = 2.1f;
    [SerializeField] float sidePushDistance = 1.1f;
    [SerializeField] float hoverAbovePlayer = 0.28f;
    [SerializeField] float groundRayLength = 4f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float attackRange = 0.62f;
    [SerializeField] float attackCooldown = 1.8f;
    [SerializeField] float knockbackHorizontal = 7.5f;
    [SerializeField] float knockbackUpward = 3.2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float arriveThreshold = 0.08f;

    Rigidbody2D rb;
    Collider2D col;
    PlayerDamageable playerDamageable;
    float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        if (col != null)
            col.isTrigger = true;

        TryResolvePlayer();
    }

    void Start()
    {
        TryResolvePlayer();
    }

    void TryResolvePlayer()
    {
        if (playerTarget == null)
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null)
                playerGo = GameObject.Find("Player");
            if (playerGo != null)
                playerTarget = playerGo.transform;
        }

        if (playerTarget != null)
            playerDamageable = playerTarget.GetComponent<PlayerDamageable>();
    }

    void FixedUpdate()
    {
        if (playerTarget == null)
            TryResolvePlayer();
        if (playerTarget == null)
            return;

        if (playerDamageable == null)
            playerDamageable = playerTarget.GetComponent<PlayerDamageable>();

        Vector2 playerPos = playerTarget.position;
        Vector2 offset = Vector2.zero;

        Vector2 rayOrigin = playerPos + Vector2.up * 0.15f;
        var hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, groundRayLength);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        RaycastHit2D groundHit = default;
        bool foundGround = false;
        foreach (var h in hits)
        {
            if (h.collider == null)
                continue;
            if (h.collider.CompareTag("Ground"))
            {
                groundHit = h;
                foundGround = true;
                break;
            }
        }

        if (foundGround)
        {
            Bounds b = groundHit.collider.bounds;
            float distLeft = playerPos.x - b.min.x;
            float distRight = b.max.x - playerPos.x;
            float pushSign = distLeft < distRight ? -1f : 1f;
            offset = new Vector2(pushSign * sidePushDistance, hoverAbovePlayer);
        }
        else
        {
            float toward = Mathf.Sign(playerPos.x - rb.position.x);
            if (toward == 0f) toward = 1f;
            offset = new Vector2(toward * sidePushDistance * 0.6f, hoverAbovePlayer);
        }

        Vector2 goal = playerPos + offset;
        Vector2 delta = goal - rb.position;
        if (delta.sqrMagnitude > arriveThreshold * arriveThreshold)
        {
            Vector2 dir = delta.normalized;
            rb.linearVelocity = dir * chaseSpeed;
        }
        else
            rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.flipX = playerPos.x < rb.position.x;

        TryAttack(playerPos);
    }

    void TryAttack(Vector2 playerPos)
    {
        if (playerDamageable == null || Time.fixedTime < nextAttackTime)
            return;

        if (Vector2.Distance(rb.position, playerPos) > attackRange)
            return;

        Vector2 fromGhost = playerPos - rb.position;
        if (fromGhost.sqrMagnitude < 0.0001f)
            fromGhost = Vector2.right;
        fromGhost.Normalize();

        Vector2 knockback = new Vector2(fromGhost.x * knockbackHorizontal, knockbackUpward);
        playerDamageable.TakeHit(knockback, attackDamage);
        nextAttackTime = Time.fixedTime + attackCooldown;
    }
}
