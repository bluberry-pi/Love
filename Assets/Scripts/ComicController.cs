using UnityEngine;
using TMPro;
using System.Collections;

public class ComicController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;

        public float typeSpeed = 0.03f;
    }

    [System.Serializable]
    public class ComicPanel
    {
        [Header("Camera")]
        public Transform cameraPoint;
        public float cameraSize = 5f;
        public float cameraMoveTime = 0.8f;

        [Header("Dialogue")]
        public TMP_Text dialogueText;

        public DialogueLine[] dialogueLines;

        [Header("After Panel")]
        public float waitAfterPanel = 1f;
    }

    [Header("References")]
    public Camera mainCamera;

    [Header("Comic Panels")]
    public ComicPanel[] panels;

    private bool isTyping = false;
    private bool skipTyping = false;


    void Start()
    {
        // Hide all dialogue initially
        foreach (ComicPanel panel in panels)
        {
            if (panel.dialogueText != null)
            {
                panel.dialogueText.gameObject.SetActive(false);
            }
        }

        StartCoroutine(PlayComic());
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Finish current line immediately
                skipTyping = true;
            }
        }
    }


    IEnumerator PlayComic()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            ComicPanel panel = panels[i];

            // Show this panel's dialogue
            panel.dialogueText.gameObject.SetActive(true);

            // Move camera to panel
            yield return StartCoroutine(MoveCamera(panel));

            // Play every dialogue line in this panel
            for (int j = 0; j < panel.dialogueLines.Length; j++)
            {
                yield return StartCoroutine(
                    TypeText(
                        panel.dialogueText,
                        panel.dialogueLines[j].text,
                        panel.dialogueLines[j].typeSpeed
                    )
                );

                // Wait for LMB or Space
                yield return StartCoroutine(WaitForInput());
            }

            // Hide dialogue
            panel.dialogueText.gameObject.SetActive(false);

            // Wait after panel
            yield return new WaitForSeconds(panel.waitAfterPanel);
        }

        // After the FINAL panel's wait time
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1
        );
    }

    IEnumerator TypeText(TMP_Text textObject, string text, float speed)
    {
        isTyping = true;
        skipTyping = false;

        textObject.text = "";

        foreach (char letter in text)
        {
            if (skipTyping)
            {
                textObject.text = text;
                break;
            }

            textObject.text += letter;

            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
        skipTyping = false;
    }


    IEnumerator WaitForInput()
    {
        bool pressed = false;

        while (!pressed)
        {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                pressed = true;
            }

            yield return null;
        }
    }


    IEnumerator MoveCamera(ComicPanel panel)
    {
        Vector3 startPosition = mainCamera.transform.position;

        Vector3 targetPosition = new Vector3(
            panel.cameraPoint.position.x,
            panel.cameraPoint.position.y,
            mainCamera.transform.position.z
        );

        float startSize = mainCamera.orthographicSize;

        float elapsed = 0f;

        while (elapsed < panel.cameraMoveTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / panel.cameraMoveTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            mainCamera.orthographicSize =
                Mathf.Lerp(
                    startSize,
                    panel.cameraSize,
                    t
                );

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = panel.cameraSize;
    }
}