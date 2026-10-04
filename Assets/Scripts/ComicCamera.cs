using UnityEngine;
using System.Collections;

public class ComicCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraPoint
    {
        public Transform point;

        [Header("Camera")]
        public float cameraSize = 5f;

        [Header("Movement")]
        public float moveTime = 0.8f;
    }

    public Camera cam;

    public CameraPoint[] points;

    public IEnumerator MoveToPoint(int index)
    {
        if (index < 0 || index >= points.Length)
            yield break;

        CameraPoint target = points[index];

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            target.point.position.x,
            target.point.position.y,
            transform.position.z
        );

        float startSize = cam.orthographicSize;

        float elapsed = 0f;

        while (elapsed < target.moveTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / target.moveTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            // Move camera
            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            // Change camera size
            cam.orthographicSize = Mathf.Lerp(
                startSize,
                target.cameraSize,
                t
            );

            yield return null;
        }

        transform.position = targetPosition;

        cam.orthographicSize = target.cameraSize;
    }
}