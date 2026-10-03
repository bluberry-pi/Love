using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Wobble")]
    [Tooltip("Drag the BODY SPRITE child here (NOT the root enemy object)")]
    public Transform bodySprite;
    public float wobbleTiltAngle = 15f;
    public float wobbleInterval  = 0.15f;

    [Header("Death")]
    [Tooltip("Blood splash animation prefab — spawned on death, should destroy itself after its animation plays")]
    public GameObject bloodSplashPrefab;

    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;

    private float wobbleTimer = 0f;
    private float wobbleSide  = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // Flip root scale based on horizontal direction
        if (dir.x != 0)
        {
            float scaleX = Mathf.Abs(transform.localScale.x) * (dir.x > 0 ? 1f : -1f);
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }

        // Keep root rotation clean so the body sprite drives the tilt
        transform.rotation = Quaternion.identity;

        // Wobble — same logic as player
        bool isMoving = dir.sqrMagnitude > 0.01f;
        if (isMoving)
        {
            wobbleTimer += Time.deltaTime;
            if (wobbleTimer >= wobbleInterval)
            {
                wobbleTimer = 0f;
                wobbleSide  = -wobbleSide;
            }
        }
        else
        {
            wobbleTimer = 0f;
            wobbleSide  = 1f;
        }

        if (bodySprite != null)
        {
            float targetZ = isMoving ? wobbleSide * wobbleTiltAngle : 0f;
            bodySprite.rotation = Quaternion.Euler(0f, 0f, targetZ);
        }
    }

    public void TakeHit()
    {
        if (isDead) return;
        isDead = true;

        // Reset body sprite tilt before the enemy is destroyed
        if (bodySprite != null)
            bodySprite.rotation = Quaternion.identity;

        // Spawn the blood splash prefab at this enemy's position and rotation
        if (bloodSplashPrefab != null)
        {
            GameObject splash = Instantiate(bloodSplashPrefab, transform.position, transform.rotation);

            // Grab the Animator (or Animation) so we know how long to wait before destroying the prefab
            Animator anim = splash.GetComponent<Animator>();
            if (anim != null)
            {
                // Wait for the clip length then destroy the splash
                float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
                Destroy(splash, clipLength > 0f ? clipLength : 1f);
            }
            else
            {
                // Fallback: destroy after 1 second if there is no Animator
                Destroy(splash, 1f);
            }
        }

        // Tell the KillCounter that an enemy died
        KillCounter.killCount++;
        Debug.Log("[KillCounter] Kills: " + KillCounter.killCount);

        // Destroy the enemy immediately
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage();
        }
    }
}
