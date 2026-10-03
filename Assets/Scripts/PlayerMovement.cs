using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float deathPauseTime = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentJumpForce;

    private bool hasInitialJumped = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = jumpForce;
    }

    void Update()
    {
        // Move left and right
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        // Initial jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasInitialJumped)
        {
            Jump();

            hasInitialJumped = true;
            isGrounded = false;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            currentJumpForce
        );
    }

    // Normal Ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If the player already used their initial jump,
            // touching the ground again means death.
            if (hasInitialJumped && !isDead)
            {
                Death();
                return;
            }

            isGrounded = true;
            currentJumpForce = jumpForce;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Platform
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            Platform platform = collision.GetComponent<Platform>();

            if (platform != null)
            {
                currentJumpForce = platform.jumpForce;

                platform.PlayerTouched();
            }

            Jump();
            isGrounded = false;
        }
    }

    void Death()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Game Over");

        StartCoroutine(DeathSequence());
    }

    System.Collections.IEnumerator DeathSequence()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(deathPauseTime);

        Debug.Log("Game Over");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}