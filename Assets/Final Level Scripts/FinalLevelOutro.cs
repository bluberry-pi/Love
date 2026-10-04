using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the end sequence for the Final Level.
/// Triggered automatically when the kill count reaches killTarget (default 30).
///
/// SEQUENCE:
///   1. Enemy spawner is stopped.
///   2. Player shooting is DISABLED (movement stays on).
///   3. First outro dialogue plays (typewriter, Space/LMB to advance).
///   4. Second outro dialogue plays (spawns after first finishes).
///   5. Player shooting is RE-ENABLED.
///   6. Door GameObject slides upward (Y axis) smoothly.
///
/// HOW TO SET UP IN UNITY:
/// ─────────────────────────────────────────────────────────────────
/// 1. Attach this script to any GameObject (e.g. "OutroManager").
/// 2. Assign:
///      - Enemy Spawner    → your EnemySpawner
///      - Outro Dialogue 1 → first DialogueSequence GO
///      - Outro Dialogue 2 → second DialogueSequence GO (starts after first ends)
///      - Door             → the door GameObject to move upward
///      - Player           → your Player GameObject
/// 3. Tweak killTarget, doorMoveDistance, doorMoveDuration as needed.
/// 4. Make sure KillCounter is present in the scene (it calls TriggerOutro).
/// ─────────────────────────────────────────────────────────────────
/// </summary>
public class FinalLevelOutro : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner     enemySpawner;
    [Tooltip("First dialogue that plays after enemies are cleared.")]
    public DialogueSequence outroDialogue1;
    [Tooltip("Second dialogue that plays immediately after the first finishes.")]
    public DialogueSequence outroDialogue2;
    [Tooltip("The door GameObject that will slide upward after both dialogues.")]
    public GameObject       door;
    [Tooltip("Your Player GameObject — used to disable shooting during the dialogues.")]
    public GameObject       player;

    [Header("Kill Trigger")]
    [Tooltip("How many kills trigger the outro.")]
    public int killTarget = 30;

    [Header("Door Movement")]
    [Tooltip("How many units the door moves upward (positive Y).")]
    public float doorMoveDistance = 5f;
    [Tooltip("How many seconds the door takes to reach its destination.")]
    public float doorMoveDuration = 2f;

    // ── Internal ──────────────────────────────────────────────────
    private bool outroTriggered = false;

    /// <summary>Singleton so KillCounter can reach this without a direct reference.</summary>
    public static FinalLevelOutro Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // ─────────────────────────────────────────────────────────────
    /// <summary>Called by KillCounter when killTarget is reached.</summary>
    public void TriggerOutro()
    {
        if (outroTriggered) return;
        outroTriggered = true;
        StartCoroutine(RunOutro());
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator RunOutro()
    {
        Debug.Log("[FinalLevelOutro] Kill target reached — stopping spawner.");

        // ── 1. Stop spawner (no new enemies will appear) ──────────
        if (enemySpawner != null)
            enemySpawner.StopSpawning();

        // ── 2. Wait until ALL remaining enemies are dead ──────────
        //    Player keeps shooting during this wait.
        Debug.Log("[FinalLevelOutro] Waiting for remaining enemies to be cleared...");
        yield return new WaitUntil(() => FindObjectsOfType<Enemy>().Length == 0);
        Debug.Log("[FinalLevelOutro] All enemies cleared!");

        // ── 3. NOW disable shooting ───────────────────────────────
        PlayerShooting shooting = player != null ? player.GetComponent<PlayerShooting>() : null;
        if (shooting != null)
        {
            shooting.SetInputEnabled(false);
            Debug.Log("[FinalLevelOutro] Shooting disabled for outro.");
        }

        // ── 4. First outro dialogue ───────────────────────────────
        yield return StartCoroutine(PlayDialogue(outroDialogue1));

        // ── 5. Second outro dialogue (if assigned) ────────────────
        yield return StartCoroutine(PlayDialogue(outroDialogue2));

        // ── 6. Re-enable shooting after both dialogues ────────────
        if (shooting != null)
        {
            shooting.SetInputEnabled(true);
            Debug.Log("[FinalLevelOutro] Shooting re-enabled.");
        }

        // ── 7. Slide door upward ──────────────────────────────────
        if (door != null)
        {
            Debug.Log("[FinalLevelOutro] Moving door upward...");
            yield return StartCoroutine(MoveDoor());
        }

        Debug.Log("[FinalLevelOutro] Outro complete.");
    }

    // ─────────────────────────────────────────────────────────────
    /// <summary>Plays a DialogueSequence and waits for it to finish. Safe to call with null.</summary>
    IEnumerator PlayDialogue(DialogueSequence dialogue)
    {
        if (dialogue == null) yield break;

        bool done = false;
        dialogue.OnSequenceComplete = () => done = true;
        dialogue.StartSequence();
        yield return new WaitUntil(() => done);
    }

    // ─────────────────────────────────────────────────────────────
    IEnumerator MoveDoor()
    {
        Vector3 startPos = door.transform.position;
        Vector3 endPos   = startPos + new Vector3(0f, doorMoveDistance, 0f);

        float elapsed = 0f;
        while (elapsed < doorMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t       = Mathf.Clamp01(elapsed / doorMoveDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);   // ease-in-out
            door.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        door.transform.position = endPos;  // snap to exact end position
    }
}
