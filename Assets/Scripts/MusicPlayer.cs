using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource music;
    static MusicPlayer instance;

    void Awake()
    {
        // If a music player already exists, remove this duplicate
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);   // keeps playing across levels
        music.loop = true;               // loops forever
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
            music.Stop();                // no music in build index 0
        else if (!music.isPlaying)
            music.Play();                // starts at index 1, then keeps going
    }
}