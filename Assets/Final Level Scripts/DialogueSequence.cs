using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// One line of dialogue. Fully configurable per-entry in the Inspector.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;

    [Tooltip("Delay in seconds between each character. Smaller = faster.\n0.03 = fast  |  0.08 = medium  |  0.15 = slow")]
    public float typeSpeed = 0.05f;
}

/// <summary>
/// Typewriter-style dialogue sequence.
///
/// HOW TO SET UP IN UNITY:
/// ─────────────────────────────────────────────────────────────────
/// 1. Create a Canvas → add a Panel child (your dialogue box).
/// 2. Inside the Panel, add a TextMeshPro - Text (UI) component.
/// 3. Attach this script to any GO (e.g. IntroManager or the Panel).
/// 4. Assign:
///      - Dialogue Panel  → the Panel GameObject
///      - Dialogue Text   → the TMP Text component
/// 5. Add as many lines as you want in the "Dialogue Lines" array.
///    Each line has its own text and typeSpeed.
/// 6. Call StartSequence() from FinalLevelIntro (already wired up).
///
/// CONTROLS:
///   Space OR Left Click during typing  → instantly show full line
///   Space OR Left Click after typing   → advance to next line
/// ─────────────────────────────────────────────────────────────────
/// </summary>
public class DialogueSequence : MonoBehaviour
{
    [Header("UI References")]
    public GameObject      dialoguePanel;
    public TextMeshProUGUI dialogueText;
    [Tooltip("The Canvas GameObject to destroy after the last line")]
    public GameObject      canvasToDestroy;

    [Header("Dialogue Lines")]
    public DialogueLine[] lines;

    /// <summary>Fired when all lines are done. FinalLevelIntro subscribes to this.</summary>
    public System.Action OnSequenceComplete;

    // ── Internal ──────────────────────────────────────────────────
    private bool typingDone      = false;   // current line fully typed?
    private bool advancePressed  = false;   // player pressed space/click?

    // ─────────────────────────────────────────────────────────────
    public void StartSequence()
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("[DialogueSequence] No lines assigned — skipping dialogue.");
            OnSequenceComplete?.Invoke();
            return;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        StartCoroutine(RunSequence());
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator RunSequence()
    {
        foreach (DialogueLine line in lines)
        {
            typingDone     = false;
            advancePressed = false;

            // Type out the line
            yield return StartCoroutine(TypeLine(line));

            typingDone = true;

            // Wait for Space or mouse click to advance
            yield return new WaitUntil(() => advancePressed);
            advancePressed = false;
        }

        // All lines done — destroy the canvas
        if (canvasToDestroy != null)
            Destroy(canvasToDestroy);
        else if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        OnSequenceComplete?.Invoke();
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator TypeLine(DialogueLine line)
    {
        if (dialogueText != null) dialogueText.text = "";

        float delay = line.typeSpeed;

        foreach (char c in line.text)
        {
            // Player pressed advance mid-typing → snap to full text
            if (advancePressed)
            {
                advancePressed = false;           // consume the press for skip, not advance
                if (dialogueText != null) dialogueText.text = line.text;
                yield break;
            }

            if (dialogueText != null) dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }

    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            advancePressed = true;
        }
    }
}
