using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Platforms")]
    public GameObject[] platforms;

    [Header("Secret Platforms")]
    public SecretPlatform[] secretPlatforms;

    [Header("Next Scene")]
    public float nextScenePause = 1f;

    private bool secretPlatformsActivated = false;
    private bool gameFinished = false;

    void Update()
    {
        if (gameFinished)
            return;

        // Check normal platforms
        if (!secretPlatformsActivated && AllPlatformsDestroyed())
        {
            if (secretPlatforms.Length > 0)
            {
                secretPlatformsActivated = true;

                foreach (SecretPlatform platform in secretPlatforms)
                {
                    if (platform != null)
                    {
                        platform.ActivatePlatform();
                    }
                }
            }
            else
            {
                gameFinished = true;
                StartCoroutine(NextScene());
            }
        }

        // Check secret platforms
        if (secretPlatformsActivated && AllSecretPlatformsDestroyed())
        {
            gameFinished = true;
            StartCoroutine(NextScene());
        }
    }

    bool AllPlatformsDestroyed()
    {
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                return false;
            }
        }

        return true;
    }

    bool AllSecretPlatformsDestroyed()
    {
        foreach (SecretPlatform platform in secretPlatforms)
        {
            if (platform != null)
            {
                return false;
            }
        }

        return true;
    }

    System.Collections.IEnumerator NextScene()
    {
        Time.timeScale = 0f;

        Debug.Log("Next Scene");

        yield return new WaitForSecondsRealtime(nextScenePause);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}