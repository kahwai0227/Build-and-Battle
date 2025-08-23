using UnityEngine;

public class Barracks : Building
{
    // Barracks-specific properties and logic
    public GameObject unitPrefab;
    public Transform spawnPoint;
    public int unitCost = 30;
    public float trainTime = 5f;
    public bool isTraining = false;

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

    public void TrainUnit()
    {
        Debug.Log("Barracks isConstructed: " + isConstructed);
        Debug.Log("Barracks isTraining: " + isTraining);

        if (isTraining)
        {
            Debug.Log("Cannot train unit: Barracks is already training.");
            return;
        }

        if (!isConstructed)
        {
            Debug.Log("Cannot train unit: Barracks is not constructed.");
            return;
        }

        Debug.Log("Training unit...");

        if (ResourceManager.Instance.gold >= unitCost)
        {
            ResourceManager.Instance.SpendGold(unitCost);
            StartCoroutine(TrainUnitCoroutine());
        }
        else
        {
            Debug.Log("Not enough gold to train unit!");
        }
    }

    private System.Collections.IEnumerator TrainUnitCoroutine()
    {
        isTraining = true;
        yield return new WaitForSeconds(trainTime);

        float spawnRadius = 3f; // Distance from barracks
        float checkRadius = 1f; // Unit size
        int steps = 12; // 360 / 30 degrees

        Vector3 center = spawnPoint != null ? spawnPoint.position : transform.position;
        Vector3 spawnPos = center + Vector3.right * spawnRadius; // Default position

        for (int i = 0; i < steps; i++)
        {
            float angle = i * (360f / steps) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = center + dir * spawnRadius;

            Collider[] hits = Physics.OverlapSphere(candidate, checkRadius, LayerMask.GetMask("Unit"));
            if (hits.Length == 0)
            {
                spawnPos = candidate;
                break;
            }
        }

        Instantiate(unitPrefab, spawnPos, Quaternion.identity);

        isTraining = false;
    }

    protected override void DestroyBuilding()
    {
        Debug.Log("Barracks has been destroyed!");
        base.DestroyBuilding();
    }
}