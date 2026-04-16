using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamageable : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    [SerializeField] float hitInvincibilityDuration = 0.85f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

    int currentHealth;
    float invincibleUntil;
    PlayerFallRespawn fallRespawn;
    PlayerController playerController;
    PlayerInput playerInput;
    bool deathRoutineRunning;

    public bool DeathSequenceActive => deathRoutineRunning;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
        fallRespawn = GetComponent<PlayerFallRespawn>();
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        currentHealth = maxHealth;
    }

    public void TakeHit(Vector2 knockback, int damage)
    {
        if (deathRoutineRunning)
            return;
        if (Time.time < invincibleUntil)
            return;

        invincibleUntil = Time.time + hitInvincibilityDuration;
        if (rb != null)
            rb.linearVelocity = knockback;

        currentHealth -= damage;

        if (currentHealth <= 0 && fallRespawn != null)
        {
            StartCoroutine(DeathThenRespawnRoutine());
            return;
        }
    }

    IEnumerator DeathThenRespawnRoutine()
    {
        deathRoutineRunning = true;

        if (playerController != null)
            playerController.enabled = false;
        if (playerInput != null)
            playerInput.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (animator != null)
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("death");
        }

        const float waitCap = 5f;
        float waited = 0f;
        bool sawDeathState = false;

        yield return null;

        while (waited < waitCap)
        {
            if (animator != null)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                if (info.IsName("player_death"))
                {
                    sawDeathState = true;
                    if (info.normalizedTime >= 1f)
                        break;
                }
            }
            else
            {
                break;
            }

            waited += Time.deltaTime;
            yield return null;
        }

        if (animator == null || !sawDeathState)
            yield return new WaitForSeconds(0.25f);

        currentHealth = maxHealth;
        invincibleUntil = Time.time + hitInvincibilityDuration;
        fallRespawn.Respawn();

        if (animator != null)
        {
            animator.ResetTrigger("death");
            animator.Play("Idle_pose", 0, 0f);
        }

        if (playerController != null)
            playerController.enabled = true;
        if (playerInput != null)
            playerInput.enabled = true;

        deathRoutineRunning = false;
    }
}
