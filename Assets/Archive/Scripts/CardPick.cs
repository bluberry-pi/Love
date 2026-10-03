using UnityEngine;

public class CardPick : MonoBehaviour
{
    [Header("Hover")]
    public float hoverScale = 1.15f;
    public float hoverHeight = 0.5f;
    public float hoverForward = -0.5f;

    [Header("Movement")]
    public float moveSpeed = 10f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Camera cam;
    private bool used;
    public float flySpeed = 15f;

    void Start()
    {
        cam = Camera.main;

        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (used)
        {
            transform.position += Vector3.up * flySpeed * Time.deltaTime;
            transform.Rotate(0, 0, 360f * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 4f);

            if (transform.localScale.x < 0.05f) gameObject.SetActive(false);
            return;
        }
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

        bool hoveringThisCard =
            hit != null &&
            hit.transform == transform &&
            hit.CompareTag("Card");

        if (hoveringThisCard)
        {
            HoverCard();
        }
        else
        {
            ReturnCard();
        }
    }

    void HoverCard()
    {
        // Bigger
        Vector3 targetScale = originalScale * hoverScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * moveSpeed
        );

        // Move UP + FORWARD
        Vector3 targetPosition = originalPosition;

        targetPosition.y += hoverHeight;
        targetPosition.z += hoverForward;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    void ReturnCard()
    {
        // Return to original size
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale,
            Time.deltaTime * moveSpeed
        );

        // Return to original position
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition,
            Time.deltaTime * moveSpeed
        );
    }
    public void Use()
    {
        used = true;
        GetComponent<Collider2D>().enabled = false; // can't click it again
    }
}