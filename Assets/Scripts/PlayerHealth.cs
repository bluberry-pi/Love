using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invincibility after hit")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;



    [Header("Flash on Damage")]
    public SpriteRenderer[] flashRenderers;
    public float flashInterval = 0.1f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        currentHealth--;
        Debug.Log($"[PlayerHealth] Hit! HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Flash the sprite renderers
        float elapsed = 0f;
        bool visible = false;
        while (elapsed < invincibilityDuration)
        {
            visible = !visible;
            foreach (var sr in flashRenderers)
                if (sr != null) sr.enabled = visible;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // Make sure renderers are visible again
        foreach (var sr in flashRenderers)
            if (sr != null) sr.enabled = true;

        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("[PlayerHealth] Player died! Game Over.");
    }

    public int GetHealth() => currentHealth;
}
