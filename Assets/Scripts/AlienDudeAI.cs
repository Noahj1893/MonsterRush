using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlienDudeAI : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    [SerializeField] float patrolSpeed = 1.4f;
    [SerializeField] float edgePadding = 0.25f;
    [SerializeField] float attackRange = 0.85f;
    [SerializeField] float attackCooldown = 1.25f;
    [SerializeField] float knockbackHorizontal = 5f;
    [SerializeField] float knockbackUpward = 2.2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float platformRayLength = 12f;
    [SerializeField] float footProbeRadius = 0.35f;
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    PlayerDamageable playerDamageable;
    Collider2D patrolPlatform;
    float leftLimit;
    float rightLimit;
    float nextAttackTime;
    int patrolDirection = 1;
    bool hasPlatformLimits;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        rb.freezeRotation = true;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
    }

    void Start()
    {
        ResolvePlayer();
        ResolvePlatformLimits();
    }

    void FixedUpdate()
    {
        if (!hasPlatformLimits)
            ResolvePlatformLimits();

        Patrol();
        TryAttack();
    }

    void ResolvePlayer()
    {
        if (playerTarget == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
                playerObj = GameObject.Find("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        if (playerTarget != null)
            playerDamageable = playerTarget.GetComponent<PlayerDamageable>();
    }

    static bool IsPlatformCollider(Collider2D c)
    {
        if (c == null || !c.enabled || c.isTrigger)
            return false;
        if (c.CompareTag("Ground"))
            return true;
        string n = c.gameObject.name;
        return n.IndexOf("platform", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static Collider2D GetColliderUnderFeet(Vector2 worldPos, float rayLen, float probeR, Rigidbody2D ignoreRb)
    {
        Vector2 origin = worldPos + Vector2.up * 0.15f;
        var hits = Physics2D.RaycastAll(origin, Vector2.down, rayLen);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            if (h.collider == null)
                continue;
            if (ignoreRb != null && h.collider.attachedRigidbody == ignoreRb)
                continue;
            if (IsPlatformCollider(h.collider))
                return h.collider;
        }

        var overlaps = Physics2D.OverlapCircleAll(origin, probeR);
        float bestY = float.NegativeInfinity;
        Collider2D best = null;
        foreach (var c in overlaps)
        {
            if (c == null)
                continue;
            if (ignoreRb != null && c.attachedRigidbody == ignoreRb)
                continue;
            if (!IsPlatformCollider(c))
                continue;
            if (c.bounds.max.y > bestY)
            {
                bestY = c.bounds.max.y;
                best = c;
            }
        }

        return best;
    }

    static bool SameStandingSurface(Collider2D underPlayer, Collider2D alienPlatform)
    {
        if (underPlayer == null || alienPlatform == null)
            return false;
        if (underPlayer == alienPlatform)
            return true;
        return underPlayer.gameObject == alienPlatform.gameObject;
    }

    void ResolvePlatformLimits()
    {
        Collider2D groundCol = GetColliderUnderFeet(transform.position, platformRayLength, footProbeRadius, rb);

        if (groundCol == null)
        {
            patrolPlatform = null;
            hasPlatformLimits = false;
            return;
        }

        patrolPlatform = groundCol;
        Bounds b = groundCol.bounds;
        leftLimit = b.min.x + edgePadding;
        rightLimit = b.max.x - edgePadding;
        hasPlatformLimits = rightLimit > leftLimit;
    }

    void Patrol()
    {
        if (!hasPlatformLimits)
            return;

        Vector2 pos = rb.position;
        float nextX = pos.x + patrolDirection * patrolSpeed * Time.fixedDeltaTime;

        if (nextX <= leftLimit)
        {
            nextX = leftLimit;
            patrolDirection = 1;
        }
        else if (nextX >= rightLimit)
        {
            nextX = rightLimit;
            patrolDirection = -1;
        }

        rb.MovePosition(new Vector2(nextX, pos.y));

        if (spriteRenderer != null)
            spriteRenderer.flipX = patrolDirection < 0;
    }

    void TryAttack()
    {
        if (Time.fixedTime < nextAttackTime)
            return;

        if (playerTarget == null || playerDamageable == null)
            ResolvePlayer();
        if (playerTarget == null || playerDamageable == null)
            return;

        if (!hasPlatformLimits || patrolPlatform == null)
            return;

        Rigidbody2D playerRb = playerTarget.GetComponent<Rigidbody2D>();
        Collider2D underPlayer = GetColliderUnderFeet(playerTarget.position, platformRayLength, footProbeRadius, playerRb);
        if (!SameStandingSurface(underPlayer, patrolPlatform))
            return;

        Vector2 delta = (Vector2)playerTarget.position - rb.position;
        if (Mathf.Abs(delta.y) > 1.5f || delta.magnitude > attackRange)
            return;

        float xDir = Mathf.Sign(delta.x);
        if (xDir == 0f)
            xDir = patrolDirection;

        Vector2 knockback = new Vector2(xDir * knockbackHorizontal, knockbackUpward);
        playerDamageable.TakeHit(knockback, attackDamage);
        nextAttackTime = Time.fixedTime + attackCooldown;
    }
}
