using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    public float jumpForce = 8f;
    public GameObject animationPrefab;

    public virtual void PlayerTouched()
    {
        if (animationPrefab != null)
        {
            Instantiate(animationPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}