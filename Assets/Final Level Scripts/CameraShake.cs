using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to the Main Camera.
/// Call CameraShake.Instance.Shake(duration, magnitude) from anywhere.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalLocalPos;

    void Awake()
    {
        Instance = this;
        originalLocalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Ease-out: shake gets weaker toward the end
            float t = elapsed / duration;
            float currentMag = magnitude * (1f - t);

            float ox = Random.Range(-1f, 1f) * currentMag;
            float oy = Random.Range(-1f, 1f) * currentMag;

            transform.localPosition = originalLocalPos + new Vector3(ox, oy, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
    }
}
