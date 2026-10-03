using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 8f;

    public GameObject animationPrefab;

    public void PlayerTouched()
    {
        // Spawn animation
        if (animationPrefab != null)
        {
            Instantiate(animationPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}