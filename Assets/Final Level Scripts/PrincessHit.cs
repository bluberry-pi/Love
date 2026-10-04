using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach this to the Princess GameObject.
/// When a bullet hits her, the screen instantly goes black and the next scene loads.
///
/// HOW TO SET UP IN UNITY:
/// ─────────────────────────────────────────────────────────────────
/// 1. Tag the Princess GameObject as "Princess".
/// 2. Make sure it has a Collider2D (set Is Trigger ✓ if using trigger).
/// 3. Attach this script to the Princess GameObject.
/// 4. Create a Canvas (Screen Space – Overlay, sort order high e.g. 99)
///      → add a child Image that covers the full canvas (stretch anchors)
///      → set the Image color to solid BLACK, Alpha = 0
///      → assign that Image to the "Black Overlay" field below.
/// 5. Set "Next Scene Name" to the exact scene name in Build Settings.
/// ─────────────────────────────────────────────────────────────────
/// </summary>
public class PrincessHit : MonoBehaviour
{
    [Header("Scene Transition")]
    [Tooltip("Exact name of the scene to load (must be in Build Settings).")]
    public string nextSceneName;

    [Header("Black Overlay")]
    [Tooltip("A full-screen black UI Image (Alpha 0 at start). Goes opaque instantly on hit.")]
    public Image blackOverlay;

    [Tooltip("Seconds to wait on the black screen before loading the next scene.")]
    public float holdDuration = 0.5f;

    private bool triggered = false;

    // ─────────────────────────────────────────────────────────────
    /// <summary>Called by Bullet when it hits this object.</summary>
    public void OnShot()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(BlackAndLoad());
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator BlackAndLoad()
    {
        // Snap the overlay to fully black instantly
        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = 1f;
            blackOverlay.color = c;
        }

        Debug.Log("[PrincessHit] Screen blacked out — loading: " + nextSceneName);

        // Short hold so the black frame is visible for at least one frame
        yield return new WaitForSeconds(holdDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("[PrincessHit] nextSceneName is empty! Set it in the Inspector.");
    }
}
