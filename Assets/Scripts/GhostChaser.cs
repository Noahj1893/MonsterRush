using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostChaser : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    [SerializeField] float chaseSpeed = 2f;
    [SerializeField] float displacementOnHit = 4f;
    Rigidbody2D rb;
    PlayerDamageable playerDamageable;
    float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTarget = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTarget == null) return;

        Vector3 playerPos = playerTarget.position;
        Vector2 posDiff = (playerPos - transform.position).normalized;
        
        transform.Translate(posDiff * Time.deltaTime * chaseSpeed);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.transform.CompareTag("Player")) return;
        other.GetComponent<PlayerDamageable>().TakeHit(Vector2.zero, 1);
        int rand = UnityEngine.Random.Range(0,3);
        Vector2 dir;
        switch (rand)
        {
            case 0: 
                dir = Vector2.up;
                break;
            case 1: 
                dir = Vector2.right;
                break;
            case 2: 
                dir = Vector2.left;
                break;
            default:
                dir = Vector2.up;
                break;
        }
        this.transform.Translate(dir * displacementOnHit);
    }
}
