using UnityEngine;

public class TownhallWorkerSpawner : MonoBehaviour
{
    public GameObject workerPrefab;
    public Transform spawnPoint; // Optional, can use townhall position if null
    public int workerCost = 50;

    public float spawnMargin = 2f;
    public float checkRadius = 0.75f;

    public float spawnCooldown = 10f; // 10 seconds delay
    private bool isSpawning = false;

    public void SpawnWorker()
    {
        if (isSpawning)
        {
            Debug.Log("Worker spawn is already in progress!");
            return;
        }

        if (ResourceManager.Instance.gold >= workerCost)
        {
            ResourceManager.Instance.SpendGold(workerCost);
            StartCoroutine(SpawnWorkerDelayed());
        }
        else
        {
            Debug.Log("Not enough gold to create a worker!");
        }
    }

    private System.Collections.IEnumerator SpawnWorkerDelayed()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnCooldown);

        Vector3 center = spawnPoint != null ? spawnPoint.position : transform.position;
        float townhallRadius = GetComponent<Collider>()?.bounds.extents.magnitude ?? 2f;

        Vector3 spawnPos = FindFreeBesidePoint(center, townhallRadius, spawnMargin);

        Instantiate(workerPrefab, spawnPos, Quaternion.identity);

        isSpawning = false;
    }

    private Vector3 FindFreeBesidePoint(Vector3 targetPosition, float targetRadius, float margin = 2f)
    {
        int steps = 36; // 360 / 10 degrees
        for (int i = 0; i < steps; i++)
        {
            float angle = i * 10 * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = targetPosition + dir * (targetRadius + margin);

            Collider[] hits = Physics.OverlapSphere(candidate, checkRadius, LayerMask.GetMask("Worker"));
            if (hits.Length == 0)
                return candidate;
        }
        // If all are occupied, just use the first direction
        return targetPosition + Vector3.right * (targetRadius + margin);
    }
}