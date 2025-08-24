using UnityEngine;
using System.Collections.Generic;

public class UnitTrainer : MonoBehaviour
{
    private List<Barracks> barracksList = new List<Barracks>();
    private UnitController unitController;

    void Start()
    {
        // Find the UnitController in the scene
        unitController = FindFirstObjectByType<UnitController>();
    }

    public void RegisterBarracks(Barracks barracks)
    {
        if (!barracksList.Contains(barracks))
            barracksList.Add(barracks);
    }

    public void UnregisterBarracks(Barracks barracks)
    {
        barracksList.Remove(barracks);
    }

    public void TrainUnit(string unitType)
    {
        foreach (var barracks in barracksList)
        {
            if (barracks.isConstructed && !barracks.isTraining)
            {
                barracks.TrainUnit(unitType); // Train the unit
                return;
            }
        }
        Debug.Log("No available barracks to train unit!");
    }

    public void OnUnitTrained(GameObject trainedUnit)
    {
        if (trainedUnit != null && unitController != null)
        {
            // Add the trained unit to the UnitController's unit list
            Unit unit = trainedUnit.GetComponent<Unit>();
            if (unit != null)
            {
                unitController.units.Add(unit);
                Debug.Log($"Unit added to UnitController: {unit.name}");
            }
        }
    }
}