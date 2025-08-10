using UnityEngine;

public enum ResourceType { Wood, Gold }

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.Wood;
    public int resourceAmount = 100;
    public int collectPerTrip = 10;

    public bool Collect()
    {
        if (resourceAmount >= collectPerTrip)
        {
            resourceAmount -= collectPerTrip;
            return true;
        }
        else if (resourceAmount > 0)
        {
            resourceAmount = 0;
            return true;
        }
        return false;
    }

    public bool IsDepleted()
    {
        return resourceAmount <= 0;
    }

    void OnMouseDown()
    {
        WorkerDrag[] workers = FindObjectsByType<WorkerDrag>(FindObjectsSortMode.None);
        foreach (var worker in workers)
        {
            if (!worker.isBusy)
            {
                worker.AssignResource(this);
                break;
            }
        }
    }
}