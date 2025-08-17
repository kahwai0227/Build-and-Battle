using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Assign all variants in Inspector
    public Transform[] spawnPoints;
    public GameObject townhall;
    public float timeBetweenWaves = 20f;
    public float spawnDistance = 500f;
    public int enemiesPerWave = 3;
    public int totalWaves = 5; // Set how many waves for a win

    private int currentWave = 0;
    private int spawnedEnemies = 0;
    private int defeatedEnemies = 0;
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
            for (int i = 0; i < enemiesPerWave + currentWave; i++)
            {
                // Spawn at random position 500 units from townhall
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                Vector3 spawnPos = townhall.transform.position + dir * spawnDistance;

                // Pick a random enemy variant
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);

                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.spawner = this; // Add a reference to the spawner
                }
                spawnedEnemies++;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void SpawnEnemy()
    {
        if (townhall == null)
        {
            Debug.LogWarning("Townhall not assigned!");
            return;
        }

        // Pick a random angle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        Vector3 spawnPos = townhall.transform.position + dir * spawnDistance;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // Call this from Enemy when destroyed
    public void OnEnemyDefeated()
    {
        defeatedEnemies++;
        if (currentWave >= totalWaves && defeatedEnemies >= spawnedEnemies)
        {
            Debug.Log("You Win! All enemy waves defeated.");
            GameManager.Instance.GameOver(true);
        }
    }
}