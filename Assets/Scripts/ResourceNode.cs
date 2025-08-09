using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int resourceAmount = 100;

    public void Collect(int amount)
    {
        resourceAmount -= amount;
        if (resourceAmount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        // Find the first idle worker and assign this resource
        WorkerDrag[] workers = FindObjectsOfType<WorkerDrag>();
        foreach (var worker in workers)
        {
            if (worker.state == WorkerDrag.WorkerState.Idle)
            {
                worker.AssignResource(this);
                break;
            }
        }
    }
}