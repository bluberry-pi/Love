using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    public float jumpForce = 8f;
    public GameObject animationPrefab;
    public AudioClip platformTouched;


    public virtual void PlayerTouched()
    {
        if (animationPrefab != null)
        {
            Instantiate(animationPrefab, transform.position, Quaternion.identity);
        }
        SoundFXManager.instance.PlaySoundFXClip(platformTouched, transform, 1f);
        Destroy(gameObject);
    }
}