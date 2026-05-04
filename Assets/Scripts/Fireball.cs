// Ethan Le (5/4/2026):
using UnityEngine; 

/** 
 * Script to handle Fireball prefab (position, spin rotation, visuals).
**/
public class Fireball : MonoBehaviour
{
    public float launchSpeed = 4f; // Speed of the fireball. 
    public float burnTime = 3f; // Burn length. 

    void Update()
    {
        // Move the launched fireball. 
        transform.Translate(Vector2.right * launchSpeed * Time.deltaTime); 
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        EnemyHealth enemy = collider.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            int dmg = 1; // Base damage of a fireball. 
            
            // If enemy is weak to fire, do double dmaage. 
            if (enemy.enemyType == EnemyType.Ice)
            {
                dmg *= 2; 
            }

            enemy.TakeHit(dmg); // Call function in EnemyHealth.cs class to perform the damage output. 
            enemy.ApplyBurn(burnTime); // Burn the enemy for specified length. 

            Destroy (gameObject); // Get rid of the fireball visual once enemy has been hi. 
        }
    }
}