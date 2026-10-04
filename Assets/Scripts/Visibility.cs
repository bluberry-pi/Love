using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visibility.cs
/// Attach to any GameObject in a scene. Repeatedly fades the screen to black
/// (or any chosen colour) and back again. All timings and speeds are exposed
/// in the Inspector so you can tweak them without touching code.
///
/// Requirements:
///   • Creates its own fullscreen Canvas + Image overlay at runtime – no
///     manual UI setup needed.
///   • Works in any scene.
/// </summary>
public class Visibility : MonoBehaviour
{
    [Header("Fade Colour")]
    [Tooltip("The colour the screen fades TO (alpha is overridden by the fade).")]
    public Color fadeColour = Color.black;

    [Header("Timing (seconds)")]
    [Tooltip("How long the screen takes to go from fully visible → fully faded.")]
    [Min(0.01f)]
    public float fadeOutDuration = 1f;

    [Tooltip("How long the screen stays fully faded before fading back in.")]
    [Min(0f)]
    public float holdDuration = 0.5f;

    [Tooltip("How long the screen takes to go from fully faded → fully visible.")]
    [Min(0.01f)]
    public float fadeInDuration = 1f;

    [Tooltip("How long the screen stays fully visible before the next fade-out starts.")]
    [Min(0f)]
    public float visibleDuration = 2f;

    [Header("Options")]
    [Tooltip("Start the fading loop automatically when the scene loads.")]
    public bool playOnAwake = true;

    [Tooltip("How many full fade cycles to run. Set to 0 for infinite looping.")]
    [Min(0)]
    public int loopCount = 0;

    // ── Private state ────────────────────────────────────────────────────────
    private Image  _overlay;
    private Canvas _canvas;
    private Coroutine _fadeCoroutine;
    private bool _isRunning;

    // ── Unity lifecycle ──────────────────────────────────────────────────────
    private void Awake()
    {
        CreateOverlay();

        if (playOnAwake)
            StartFading();
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Starts (or restarts) the fading loop.</summary>
    public void StartFading()
    {
        if (_isRunning)
            StopFading();

        _fadeCoroutine = StartCoroutine(FadeLoop());
    }

    /// <summary>Stops the fading loop immediately and clears the overlay.</summary>
    public void StopFading()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _isRunning = false;
        SetOverlayAlpha(0f);
    }

    // ── Internal helpers ─────────────────────────────────────────────────────

    private IEnumerator FadeLoop()
    {
        _isRunning = true;
        int cyclesDone = 0;

        while (_isRunning)
        {
            // ── Wait while game is fully visible ────────────────────────────
            if (visibleDuration > 0f)
                yield return new WaitForSeconds(visibleDuration);

            // ── Fade OUT (visible → black) ──────────────────────────────────
            yield return Fade(0f, 1f, fadeOutDuration);

            // ── Hold fully faded ────────────────────────────────────────────
            if (holdDuration > 0f)
                yield return new WaitForSeconds(holdDuration);

            // ── Fade IN (black → visible) ───────────────────────────────────
            yield return Fade(1f, 0f, fadeInDuration);

            // ── Loop control ────────────────────────────────────────────────
            if (loopCount > 0)
            {
                cyclesDone++;
                if (cyclesDone >= loopCount)
                {
                    _isRunning = false;
                    yield break;
                }
            }
        }
    }

    /// <summary>Smoothly interpolates the overlay alpha from <c>from</c> to <c>to</c> over <c>duration</c> seconds.</summary>
    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Use SmoothStep for a more cinematic, ease-in-out feel.
            SetOverlayAlpha(Mathf.SmoothStep(from, to, t));
            yield return null;
        }

        // Guarantee we land exactly on the target value.
        SetOverlayAlpha(to);
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (_overlay == null) return;
        Color c = fadeColour;
        c.a = alpha;
        _overlay.color = c;
    }

    /// <summary>Builds a fullscreen Canvas + Image that sits on top of everything.</summary>
    private void CreateOverlay()
    {
        // Canvas
        GameObject canvasGO = new GameObject("VisibilityOverlay_Canvas");
        DontDestroyOnLoad(canvasGO);   // survives scene loads – remove if you don't want that
        _canvas = canvasGO.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999;   // always on top

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Overlay image
        GameObject imgGO = new GameObject("VisibilityOverlay_Image");
        imgGO.transform.SetParent(canvasGO.transform, false);

        _overlay = imgGO.AddComponent<Image>();
        _overlay.raycastTarget = false; // don't block UI clicks

        // Stretch to fill the whole screen
        RectTransform rt = _overlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;

        // Start fully transparent
        SetOverlayAlpha(0f);
    }

    private void OnDestroy()
    {
        // Clean up the overlay when this component is destroyed
        if (_canvas != null)
            Destroy(_canvas.gameObject);
    }
}
