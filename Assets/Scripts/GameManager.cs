using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] platforms;
    public float pauseTime = 2f;
    public float waitBeforePause = 1.5f;

    private bool allPlatformsDestroyed = false;

    void Update()
    {
        if (!allPlatformsDestroyed && AllPlatformsDestroyed())
        {
            allPlatformsDestroyed = true;
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

    System.Collections.IEnumerator NextScene()
    {
        yield return new WaitForSecondsRealtime(waitBeforePause);
        Time.timeScale = 0f;

        //Debug.Log("All platforms destroyed!");

        yield return new WaitForSecondsRealtime(pauseTime);

        //Debug.Log("Next Scene");
    }
}