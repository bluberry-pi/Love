using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject dullscreen;
    public GameObject tutorialText;
    void Start()
    {
        Time.timeScale = 0f;
        dullscreen.SetActive(true);
        tutorialText.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            dullscreen.SetActive(false);
            tutorialText.SetActive(false);
        }
    }
}