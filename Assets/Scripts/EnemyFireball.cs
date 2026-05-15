// Ethan Le (5/4/2026):
using System.Collections; 
using UnityEngine; 

/** 
 * Script to handle Fireball prefab (position, spin rotation, visuals).
**/
public class EnemyFireball : MonoBehaviour
{
    public float launchSpeed = 4f; // Speed of the fireball. 
    public float burnTime = 3f; // Burn length. 
    Vector2 direction = Vector2.zero; // For fireball's direction. 

    public void Init(Vector2 dir)
    {
        direction = dir; // Update direction of where the fireball should go, if needed. 
    }

    void Update()
    {
        // Move the launched fireball in the specified direction. 
        transform.Translate(direction * launchSpeed * Time.deltaTime, Space.World); 

        // Begin coroutine for fireball existence onscreen:
        StartCoroutine(OnScreen()); 
    }

    IEnumerator OnScreen()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds. 

        Destroy(gameObject); // Then automatically destroy the fireball if it has not contacted an enemy. 
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDamageable player = collider.GetComponent<PlayerDamageable>();

        if (player != null)
        {
            int dmg = 1; // Base damage of a fireball. 

            player.TakeHit(Vector2.zero, dmg);
            Destroy(gameObject);
        }
    }
}