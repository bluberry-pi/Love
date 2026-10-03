using UnityEngine;

public class PlayerMovementFinal : MonoBehaviour
{
    public float speed = 5f;

    [Header("Wobble")]
    [Tooltip("Drag the BODY SPRITE child here (NOT the root player object)")]
    public Transform bodySprite;
    public float wobbleTiltAngle = 15f;
    public float wobbleInterval  = 0.15f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private float wobbleTimer = 0f;
    private float wobbleSide  = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical).normalized;

        // --- Flip: only localScale, root rotation always identity ---
        if (horizontal > 0f)
            transform.localScale = new Vector3( Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontal < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Root stays at identity — gun child inherits this, never tilts
        transform.rotation = Quaternion.identity;

        // --- Wobble: body sprite child only ---
        bool isMoving = horizontal != 0f;

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

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }
}