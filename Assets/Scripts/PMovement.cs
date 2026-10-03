using UnityEngine;

public class PMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Default Jump")]
    public float defaultJumpForce = 8f;

    [Header("Card Jump Forces")]
    public float cardJump = 8f;
    public float card1Jump = 8f;
    public float card2Jump = 8f;
    public float card3Jump = 12f;
    public float card4Jump = 15f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(defaultJumpForce);
        }
    }

    void Jump(float jumpForce)
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Card"))
        {
            float jumpForce = GetCardJumpForce(other.gameObject.name);
            Jump(jumpForce);
        }
    }

    float GetCardJumpForce(string cardName)
    {
        if (cardName == "Card")
            return cardJump;

        if (cardName == "Card (1)")
            return card1Jump;

        if (cardName == "Card (2)")
            return card2Jump;

        if (cardName == "Card (3)")
            return card3Jump;

        if (cardName == "Card (4)")
            return card4Jump;

        return defaultJumpForce;
    }

    // Detect landing
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}