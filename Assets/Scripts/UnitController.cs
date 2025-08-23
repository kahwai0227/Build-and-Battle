using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public LayerMask groundLayer;
    public float attackRadius = 10f;

    public bool isGatherMode = false;

    private BuildingPlacer buildingPlacer;

    void Start()
    {
        // Find the BuildingPlacer in the scene
        buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
    }

    void Update()
    {
        if (isGatherMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 gatherPoint = hit.point;
                GatherUnits(gatherPoint);
            }
        }

        foreach (var unit in units)
        {
            if (unit != null && unit.isIdle)
            {
                Enemy nearestEnemy = FindNearestEnemy(unit.transform.position);
                if (nearestEnemy != null)
                {
                    unit.Attack(nearestEnemy);
                }
            }
        }
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

    private void GatherUnits(Vector3 gatherPoint)
    {
        Debug.Log($"Gathering units at: {gatherPoint}");
        foreach (var unit in units)
        {
            if (unit != null)
            {
                unit.MoveTo(gatherPoint);
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
}