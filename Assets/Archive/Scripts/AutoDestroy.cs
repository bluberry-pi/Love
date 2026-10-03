using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // Destroy after the first animation clip finishes
            Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Destroy(gameObject, 1f); // fallback
        }
    }
}