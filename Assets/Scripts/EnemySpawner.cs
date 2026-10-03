using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [Tooltip("Drag your enemy prefabs here. A random one is picked each spawn.")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [Tooltip("Drag all spawn point GameObjects here.")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;       // seconds between spawns
    public int   maxEnemiesAlive = 10;     // cap so the scene isn't flooded
    public float firstSpawnDelay = 1f;     // delay before first spawn

    private int currentEnemies = 0;

    void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] No enemy prefabs assigned!");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] No spawn points assigned!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstSpawnDelay);

        while (true)
        {
            if (currentEnemies < maxEnemiesAlive)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        // Pick a random prefab and a random spawn point
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform  spawnPt = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(prefab, spawnPt.position, Quaternion.identity);

        // Track live count
        currentEnemies++;
        EnemyTracker tracker = enemy.AddComponent<EnemyTracker>();
        tracker.spawner = this;
    }

    public void OnEnemyDied()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }
}

// Tiny helper attached at runtime to notify the spawner when the enemy dies
public class EnemyTracker : MonoBehaviour
{
    public EnemySpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnEnemyDied();
    }
}
