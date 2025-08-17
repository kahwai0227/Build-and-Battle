using UnityEngine;
using System.Collections.Generic;

public class UnitTrainer : MonoBehaviour
{
    private List<Barracks> barracksList = new List<Barracks>();

    public void RegisterBarracks(Barracks barracks)
    {
        if (!barracksList.Contains(barracks))
            barracksList.Add(barracks);
    }

    public void UnregisterBarracks(Barracks barracks)
    {
        barracksList.Remove(barracks);
    }

    public void TrainUnit()
    {
        foreach (var barracks in barracksList)
        {
            if (barracks.isConstructed && !barracks.isTraining)
            {
                barracks.TrainUnit();
                return;
            }
        }
        Debug.Log("No available barracks to train unit!");
    }
}