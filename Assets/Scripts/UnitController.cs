using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public LayerMask groundLayer;
    public float attackRadius = 20f;

    public bool isGatherMode = false;

    private BuildingPlacer buildingPlacer;

    void Start()
    {
        // Find the BuildingPlacer in the scene
        buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
    }

    void Update()
    {
        // Prevent unit movement when clicking on UI elements
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

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
        if (units.Count == 0) return;

        Debug.Log($"Gathering units at: {gatherPoint}");

        // Use the first unit in the list as the pivot unit
        Unit pivotUnit = units[0];
        if (pivotUnit != null)
        {
            pivotUnit.MoveTo(gatherPoint); // Move the pivot unit to the exact gather point
        }

        // Calculate positions for the remaining units around the pivot unit
        float spacing = 2f; // Distance between units
        int unitsPerCircle = 6; // Number of units per circle layer
        int currentCircle = 1; // Start with the first circle layer
        int currentUnitIndex = 1; // Start with the second unit (index 1)

        for (int i = 1; i < units.Count; i++)
        {
            Unit unit = units[i];
            if (unit != null)
            {
                // Calculate the angle and position for the unit
                float angle = (currentUnitIndex - 1) * (360f / unitsPerCircle) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (spacing * currentCircle);
                Vector3 targetPosition = gatherPoint + offset;

                unit.MoveTo(targetPosition);

                // Update the unit index and circle layer
                currentUnitIndex++;
                if (currentUnitIndex > unitsPerCircle)
                {
                    currentUnitIndex = 1;
                    currentCircle++;
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
}