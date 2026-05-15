using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFallRespawn : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] float belowCameraExtraUnits = 1.25f;

    Vector3 spawnPosition;
    Rigidbody2D rb;
    Camera cam;
    PlayerDamageable damageable;
    ScoreUI scoreUI; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<PlayerDamageable>();
        spawnPosition = transform.position;
        cam = Camera.main;

        // Retrieve Score UI component through tag:
        scoreUI = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreUI>();  
    }

    void LateUpdate()
    {
        if (damageable != null && damageable.DeathSequenceActive)
            return;

        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return;

        float bottomWorld = cam.transform.position.y - cam.orthographicSize;
        bool fellBelowCamera = transform.position.y < bottomWorld - belowCameraExtraUnits;

        if (fellBelowCamera)
            Respawn();
    }

    public void Respawn()
    {
        if (damageable != null) // Reset UI to display full HP after dying. 
        {
            damageable.ResetHealth(); 
        }

        Vector3 pos = respawnPoint != null ? respawnPoint.position : spawnPosition;
        transform.position = pos;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
