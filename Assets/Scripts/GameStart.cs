using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject dullscreen;
    public GameObject firstTutorialText;

    void Start()
    {
        // If either object is missing, do absolutely nothing
        if (dullscreen == null || firstTutorialText == null)
            return;

        Time.timeScale = 0f;

        dullscreen.SetActive(true);
        firstTutorialText.SetActive(true);
    }

    void Update()
    {
        // If either object is missing, do absolutely nothing
        if (dullscreen == null || firstTutorialText == null)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;

            dullscreen.SetActive(false);
            firstTutorialText.SetActive(false);
        }
    }
}