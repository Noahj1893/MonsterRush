using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostChaser : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    [SerializeField] float chaseSpeed = 2f;
    [SerializeField] float displacementOnHit = 4f;
    [SerializeField] Vector2 playerOffset;
    Rigidbody2D rb;
    BoxCollider2D bc;
    SpriteRenderer sr;
    bool isVanishing = false;
    PlayerDamageable playerDamageable;
    float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
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
        if(isVanishing) return;
        if (playerTarget == null) return;

        Vector2 playerPos = playerTarget.position;
        playerPos += playerOffset;
        Vector2 thisPos = transform.position;
        Vector2 posDiff = (playerPos - thisPos).normalized;
        
        transform.Translate(posDiff * Time.deltaTime * chaseSpeed);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.transform.CompareTag("Player") || isVanishing) return;
        Displace();
    }
    public void Displace()
    {
        isVanishing = true;
        bc.enabled = false;
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
        
        StartCoroutine(Vanish(dir));
    }
    private IEnumerator Vanish(Vector2 dir)
    {
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        float elapsed = 0f;
        while(elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(startColor, endColor, elapsed);
            yield return null;
        }
        sr.color = endColor;
        this.transform.Translate(dir * displacementOnHit);
        yield return null;
        elapsed = 0f;
        while(elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(endColor, startColor, elapsed);
            yield return null;
        }
        bc.enabled = true;
        isVanishing = false;
    }
}
