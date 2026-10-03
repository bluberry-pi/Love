using UnityEngine;

public class PlatformDestroyAnimation : MonoBehaviour
{
    public float animationTime = 1f;

    void Start()
    {
        Destroy(gameObject, animationTime);
    }
}