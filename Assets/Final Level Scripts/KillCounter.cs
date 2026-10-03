using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public static int killCount = 0;

    void Awake()
    {
        killCount = 0;
    }

    public void RegisterKill()
    {
        killCount++;
        Debug.Log("[KillCounter] Kills: " + killCount);
    }
}
