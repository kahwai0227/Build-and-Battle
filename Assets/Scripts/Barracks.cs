using UnityEngine;

public class Barracks : Building
{
    public GameObject unitPrefab;
    public Transform spawnPoint; // Optional spawn point
    public int unitCost = 30;
    public float trainTime = 5f; // Time required to train a unit
    public bool isTraining = false;
    public float spawnRadius = 5f; // Radius to find an empty spawn area

    void Start()
    {
        // Register this barracks with the UnitTrainer on the GameManager object
        var trainer = FindFirstObjectByType<UnitTrainer>();
        if (trainer != null)
            trainer.RegisterBarracks(this);
    }

    void OnDestroy()
    {
        // Unregister this barracks from the UnitTrainer
        var trainer = FindFirstObjectByType<UnitTrainer>();
        if (trainer != null)
            trainer.UnregisterBarracks(this);
    }

    public bool IsTraining()
    {
        return isTraining;
    }

    public GameObject TrainUnit()
    {
        if (isTraining || unitPrefab == null)
        {
            Debug.LogWarning("Cannot train unit: Already training or no unit prefab assigned.");
            return null;
        }

        isTraining = true;
        Debug.Log("Training unit...");
        StartCoroutine(TrainUnitCoroutine());
        return null; // Temporarily return null since the unit is spawned asynchronously
    }

    private System.Collections.IEnumerator TrainUnitCoroutine()
    {
        // Wait for the training time to complete
        yield return new WaitForSeconds(trainTime);

        // Find a valid spawn position near the barracks
        Vector3 spawnPosition = FindFreeBesidePoint(transform.position, spawnRadius);
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("No valid spawn position found! Spawning at barracks position.");
            spawnPosition = transform.position; // Fallback to barracks position
        }

        // Spawn the unit
        GameObject trainedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

        isTraining = false;
        Debug.Log("Unit training complete!");

        // Notify the UnitTrainer about the trained unit
        var trainer = FindFirstObjectByType<UnitTrainer>();
        if (trainer != null && trainedUnit != null)
        {
            trainer.OnUnitTrained(trainedUnit);
        }
    }

    private Vector3 FindFreeBesidePoint(Vector3 targetPosition, float targetRadius, float margin = 1.5f)
    {
        float checkRadius = 0.75f; // Half the worker's width, adjust as needed
        int steps = 36; // 360 / 10 degrees

        for (int i = 0; i < steps; i++)
        {
            float angle = i * 10 * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = targetPosition + dir * (targetRadius + margin);

            Collider[] hits = Physics.OverlapSphere(candidate, checkRadius, LayerMask.GetMask("Unit"));
            if (hits.Length == 0)
                return candidate;
        }

        // If all are occupied, just use the first direction
        return targetPosition + Vector3.right * (targetRadius + margin);
    }

    protected override void DestroyBuilding()
    {
        Debug.Log("Barracks has been destroyed!");
        base.DestroyBuilding();
    }
}