using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the cinematic intro for the Final Level:
///   1. Player spawns above the camera (out of view)
///   2. Player falls under gravity to a land point you place in the scene
///   3. On landing -> camera shake (impact feel)
///   4. [Dialogue will go here next]
///   5. Enemy spawner starts
///
/// HOW TO SET UP IN UNITY:
/// ─────────────────────────────────────────────────────────────────
/// 1. Add CameraShake to your Main Camera.
///
/// 2. Create an empty GameObject → name it "LandPoint" → position it
///    where you want the player to land. Assign to 'landPoint' field.
///
/// 3. Add a child to LandPoint → add BoxCollider2D (Is Trigger ✓)
///    → add LandDetector script → assign this FinalLevelIntro to it.
///
/// 4. Assign your Player, EnemySpawner to the fields below.
///
/// 5. Make sure EnemySpawner's "Auto Start" is OFF (already handled).
///
/// 6. Player will be placed above the camera automatically on Start.
///    'spawnHeightAboveLand' controls how high above the land point.
/// ─────────────────────────────────────────────────────────────────
/// </summary>
public class FinalLevelIntro : MonoBehaviour
{
    [Header("References")]
    public GameObject      player;
    public Transform       landPoint;       // Empty GO you place where player lands
    public EnemySpawner   enemySpawner;    // Assign your EnemySpawner here
    public DialogueSequence dialogue;      // Assign your DialogueSequence here

    [Header("Fall Settings")]
    [Tooltip("How many units above the land point to spawn the player (out of camera view)")]
    public float spawnHeightAboveLand = 12f;

    [Header("Camera Shake on Impact")]
    public float shakeDuration  = 0.55f;
    public float shakeMagnitude = 0.35f;

    [Header("Post-Land Pause (before dialogue)")]
    [Tooltip("Small freeze after landing before the dialogue starts")]
    public float postLandPause = 0.5f;

    // ── Internal ──────────────────────────────────────────────────
    private PlayerMovementFinal playerMovement;
    private PlayerShooting      playerShooting;
    private Rigidbody2D         playerRb;
    private float               originalGravityScale = 0f;

    [Tooltip("How close (in units) to the LandPoint Y before we consider the player landed")]
    public float landThreshold = 0.6f;

    void Start()
    {
        if (player == null || landPoint == null)
        {
            Debug.LogError("[FinalLevelIntro] Player or LandPoint not assigned!");
            return;
        }

        playerMovement = player.GetComponent<PlayerMovementFinal>();
        playerShooting = player.GetComponent<PlayerShooting>();
        playerRb       = player.GetComponent<Rigidbody2D>();

        // Remember the original gravity (top-down games usually have it at 0)
        if (playerRb != null)
            originalGravityScale = playerRb.gravityScale;

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // ── 1. Place player above the land point, out of camera ──
        player.transform.position = new Vector3(
            landPoint.position.x,
            landPoint.position.y + spawnHeightAboveLand,
            player.transform.position.z
        );

        // Lock all input so player can't move/shoot during fall
        SetPlayerInputEnabled(false);

        // Make sure gravity is active and velocity is zero so it's a clean drop
        if (playerRb != null)
        {
            playerRb.gravityScale = 1f;      // ensure gravity is on
            playerRb.linearVelocity = Vector2.zero;
        }

        Debug.Log("[FinalLevelIntro] Player falling...");

        // ── 2. Wait until player Y reaches the land point Y ───────
        // Checks position every frame — no trigger/collider needed.
        yield return new WaitUntil(() =>
            player.transform.position.y <= landPoint.position.y + landThreshold
        );

        Debug.Log("[FinalLevelIntro] Landed! Camera shake...");

        // Stop player sliding after landing + restore gravity to original
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.gravityScale = originalGravityScale;
        }

        // ── 3. Camera shake (impact) ───────────────────────────────
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);

        yield return new WaitForSeconds(shakeDuration + postLandPause);

        // ── 4. Dialogue sequence ───────────────────────────────────
        if (dialogue != null)
        {
            bool dialogueDone = false;
            dialogue.OnSequenceComplete = () => dialogueDone = true;
            dialogue.StartSequence();
            yield return new WaitUntil(() => dialogueDone);
        }

        OnDialogueFinished();
    }

    /// <summary>
    /// Call this once dialogue is done to start enemy spawning and unlock the player.
    /// </summary>
    public void OnDialogueFinished()
    {
        Debug.Log("[FinalLevelIntro] Intro done — enabling player & starting enemies.");

        SetPlayerInputEnabled(true);

        if (enemySpawner != null)
            enemySpawner.StartSpawning();
        else
            Debug.LogWarning("[FinalLevelIntro] No EnemySpawner assigned — enemies won't spawn.");
    }

    void SetPlayerInputEnabled(bool enabled)
    {
        if (playerMovement != null) playerMovement.SetInputEnabled(enabled);
        if (playerShooting  != null) playerShooting.SetInputEnabled(enabled);
    }
}
