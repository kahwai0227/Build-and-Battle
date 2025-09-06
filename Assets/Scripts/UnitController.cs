using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public LayerMask groundLayer;
    public float attackRadius = 50f;

    public bool isGatherMode = false;

    private BuildingPlacer buildingPlacer;

    // Input Actions
    private InputAction touchAction;

    void Start()
    {
        // Find the BuildingPlacer in the scene
        buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
    }

    public void ToggleGatherMode()
    {
        isGatherMode = !isGatherMode;

        // Disable construction mode when gather mode is enabled
        if (buildingPlacer != null && isGatherMode)
        {
            buildingPlacer.DeselectBuilding();
        }

        Debug.Log($"Gather mode: {(isGatherMode ? "ON" : "OFF")}");
    }

    public void GatherUnits(Vector3 gatherPoint)
    {
        if (units.Count == 0) return;

        Debug.Log($"Gathering units at: {gatherPoint}");

        // Use the first unit in the list as the pivot unit
        Unit pivotUnit = units[0];
        if (pivotUnit != null)
        {
            pivotUnit.MoveTo(gatherPoint); // Move the pivot unit to the exact gather point
        }

        // Define grid parameters
        float spacing = 5f; // Distance between units
        int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(units.Count)); // Calculate the number of units per row
        int currentRow = 0;
        int currentColumn = 0;

        // Calculate positions for the remaining units in a rectangular grid
        for (int i = 1; i < units.Count; i++)
        {
            Unit unit = units[i];
            if (unit != null)
            {
                // Calculate the position in the grid
                Vector3 offset = new Vector3(currentColumn * spacing, 0, -currentRow * spacing);
                Vector3 targetPosition = gatherPoint + offset;

                unit.MoveTo(targetPosition);

                // Update column and row indices
                currentColumn++;
                if (currentColumn >= unitsPerRow)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
        }
    }

    private Enemy FindNearestEnemy(Vector3 unitPosition)
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        float closestDistance = attackRadius;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(unitPosition, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void Update()
    {
        foreach (Unit unit in units)
        {
            if (unit != null)
            {
                Enemy nearestEnemy = FindNearestEnemy(unit.transform.position);
                if (nearestEnemy != null)
                {
                    unit.Attack(nearestEnemy);
                }
            }
        }
    }
}