using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform imageTransform;
    [SerializeField] Animator animator;
    

    [Header("Projectile Attack")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform firePoint;

    [Header("Enemy Drop Attack")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] Transform[] flyPoints;

    // =========================
    // STATS
    // =========================

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Cooldowns")]
    [SerializeField] float attackCooldown = 3f;

    // =========================
    // STATE
    // =========================

    bool isAttacking;
    bool isPhaseTransitioning;

    bool isDead = false;
    enum BossAttack
    {
        Fireball,
        Charge,
        DropEnemies
    }

    BossAttack currentAttack;

    void Start()
    {
        StartCoroutine(BossBehaviorLoop());
    }

    void Update()
    {
        FacePlayer();
    }

    // =========================
    // BOSS LOOP
    // =========================

    IEnumerator BossBehaviorLoop()
    {
        while (!isDead)
        {
            if (!isAttacking)
            {
                ChooseAttack();

                yield return StartCoroutine(PerformAttack(currentAttack));

                yield return new WaitForSeconds(attackCooldown);
            }

            yield return null;
        }
    }

    // =========================
    // ATTACK SELECTION
    // =========================

    void ChooseAttack()
    {
        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                currentAttack = BossAttack.Fireball;
                break;

            case 1:
                currentAttack = BossAttack.Charge;
                break;

            case 2:
                currentAttack = BossAttack.DropEnemies;
                break;
        }
    }

    IEnumerator PerformAttack(BossAttack attack)
    {
        isAttacking = true;

        switch (attack)
        {
            case BossAttack.Fireball:
                yield return StartCoroutine(FireballAttack());
                break;

            case BossAttack.Charge:
                yield return StartCoroutine(ChargeAttack());
                break;

            case BossAttack.DropEnemies:
                yield return StartCoroutine(DropEnemyAttack());
                break;
        }

        isAttacking = false;
    }

    // =========================
    // FIREBALL ATTACK
    // =========================

    IEnumerator FireballAttack()
    {
        Debug.Log("Boss used FIREBALL");

        animator.SetTrigger("ShootFireball");

        yield return new WaitForSeconds(0.5f);

        // Spawn projectile
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        EnemyFireball fb = fireball.GetComponent<EnemyFireball>();
        fb.Init(Vector2.Normalize(new Vector2(player.position.x, player.position.y + 0.35f) - new Vector2(transform.position.x, transform.position.y)));

        yield return new WaitForSeconds(1f);
    }

    // =========================
    // CHARGE ATTACK
    // =========================

    IEnumerator ChargeAttack()
    {
        Debug.Log("Boss used CHARGE");
        animator.SetBool("Fly", true);

        Vector2 direction =
            (player.position - transform.position).normalized;

        float chargeTime = 1.5f;
        float timer = 0f;

        while (timer < chargeTime)
        {
            rb.linearVelocity = direction * 7f;

            timer += Time.deltaTime;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);
    }

    // =========================
    // DROP ENEMIES ATTACK
    // =========================

    IEnumerator DropEnemyAttack()
    {
        Debug.Log("Boss used DROP ENEMIES");
        animator.SetBool("Fly", true);

        // Fly around map
        for (int i = 0; i < flyPoints.Length; i++)
        {
            yield return StartCoroutine(MoveToPoint(flyPoints[i].position));
            
            int rand = Random.Range(0, enemyPrefabs.Length);

            // Drop enemy
            GameObject minion = Instantiate(enemyPrefabs[rand], transform.position, Quaternion.identity);
            PatrolEnemy pE = minion.GetComponent<PatrolEnemy>();
            FallingEnemy fE = minion.GetComponent<FallingEnemy>();
            if(pE) pE.setPatrol(minion.transform.position.x - 2f, minion.transform.position.x + 2f);
            if(fE) fE.setFallPoint(transform.position);


            yield return new WaitForSeconds(0.5f);
        }
        animator.SetBool("Fly", false);
    }

    // =========================
    // MOVEMENT HELPERS
    // =========================

    IEnumerator MoveToPoint(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = imageTransform.localScale;

        if (player.position.x < transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        imageTransform.localScale = scale;
    }
    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }
} 