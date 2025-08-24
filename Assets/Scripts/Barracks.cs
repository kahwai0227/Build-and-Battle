using UnityEngine;

public class Barracks : Building
{
    public GameObject meleeUnitPrefab;
    public GameObject rangedUnitPrefab;
    public GameObject tankUnitPrefab;

    public int meleeUnitCost = 30;
    public int rangedUnitCost = 40;
    public int tankUnitCost = 60;

    public Transform spawnPoint;
    public float trainTime = 5f;
    public float spawnRadius = 2f; // Radius used to find spawn position

    public bool isTraining { get; private set; } // Tracks if the barracks is training a unit

    private UnitTrainer unitTrainer;

    void Start()
    {
        // Find the UnitTrainer in the scene and register this barracks
        unitTrainer = FindFirstObjectByType<UnitTrainer>();
        if (unitTrainer != null)
        {
            unitTrainer.RegisterBarracks(this);
        }
    }

    void OnDestroy()
    {
        // Unregister this barracks from the UnitTrainer
        if (unitTrainer != null)
        {
            unitTrainer.UnregisterBarracks(this);
        }
    }

    public GameObject TrainUnit(string unitType)
    {
        GameObject unitPrefab = null;
        int unitCost = 0;

        switch (unitType)
        {
            case "Melee":
                unitPrefab = meleeUnitPrefab;
                unitCost = meleeUnitCost;
                break;
            case "Ranged":
                unitPrefab = rangedUnitPrefab;
                unitCost = rangedUnitCost;
                break;
            case "Tank":
                unitPrefab = tankUnitPrefab;
                unitCost = tankUnitCost;
                break;
        }

        // Check if the player has enough resources
        if (unitPrefab != null && ResourceManager.Instance.GetGold() >= unitCost)
        {
            // Deduct the cost and start training
            ResourceManager.Instance.SpendGold(unitCost);
            StartCoroutine(TrainUnitCoroutine(unitPrefab));
            return unitPrefab;
        }

        Debug.Log("Not enough resources to train unit!");
        return null; // Return null if training cannot start
    }

    private System.Collections.IEnumerator TrainUnitCoroutine(GameObject unitPrefab)
    {
        isTraining = true; // Set isTraining to true
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
        if (unitTrainer != null && trainedUnit != null)
        {
            unitTrainer.OnUnitTrained(trainedUnit);
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