using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 20f;
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
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
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