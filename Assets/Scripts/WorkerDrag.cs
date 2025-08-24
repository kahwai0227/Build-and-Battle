using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class WorkerDrag : MonoBehaviour
{
    public enum WorkerState { Idle, MovingToResource, Collecting, MovingToTownhall, Delivering }
    public WorkerState state = WorkerState.Idle;

    public ResourceNode assignedResource;
    public GameObject townhall;
    public float collectTime = 2f;
    public int carryAmount = 10;
    private NavMeshAgent agent;
    private bool isCarrying = false;
    public bool isBusy = false;
    public float moveSpeed = 15f;

    public Coroutine constructionCoroutine;
    public GameObject currentBuilding;
    public int lastGoldCost;
    public int lastWoodCost;
    public GameObject constructionProgressBar; // Reference to the construction progress bar object

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        townhall = GameObject.FindWithTag("Townhall");
    }

    private Vector3 GetOrFindFreeBesidePoint(Vector3 targetPosition, float targetRadius, float margin = 0.5f)
    {
        float checkRadius = 0.75f; // Half the worker's width, adjust as needed
        int steps = 36; // 360 / 10 degrees
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = float.MaxValue;

        // Try to find the closest free position around the target
        for (int i = 0; i < steps; i++)
        {
            float angle = i * 10 * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = targetPosition + dir * (targetRadius + margin);

            Collider[] hits = Physics.OverlapSphere(candidate, checkRadius, LayerMask.GetMask("Worker"));
            if (hits.Length == 0)
            {
                float distance = Vector3.Distance(transform.position, candidate);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = candidate;
                }
            }
        }

        // If no free position is found, fall back to a default beside point
        if (closestPoint == Vector3.zero)
        {
            Vector3 fallbackDirection = (transform.position - targetPosition).normalized;
            if (fallbackDirection == Vector3.zero) fallbackDirection = Vector3.right;
            closestPoint = targetPosition + fallbackDirection * (targetRadius + margin);
        }

        return closestPoint;
    }

    void Update()
    {
        switch (state)
        {
            case WorkerState.MovingToResource:
                if (assignedResource)
                {
                    float resourceRadius = assignedResource.GetComponent<Collider>()?.bounds.extents.magnitude ?? 2f;
                    Vector3 currentTarget = GetOrFindFreeBesidePoint(assignedResource.transform.position, resourceRadius);

                    // If the worker is not already moving to the closest free point, update the destination
                    if (Vector3.Distance(agent.destination, currentTarget) > 0.1f)
                    {
                        agent.SetDestination(currentTarget);
                    }

                    // Check if the worker has reached the free point
                    if (Vector3.Distance(transform.position, currentTarget) < 2.0f)
                    {
                        state = WorkerState.Collecting;
                        StartCoroutine(CollectResource());
                    }
                }
                break;

            case WorkerState.MovingToTownhall:
                if (townhall)
                {
                    float townhallRadius = townhall.GetComponent<Collider>()?.bounds.extents.magnitude ?? 2f;
                    Vector3 currentTarget = GetOrFindFreeBesidePoint(townhall.transform.position, townhallRadius);

                    // If the worker is not already moving to the closest free point, update the destination
                    if (Vector3.Distance(agent.destination, currentTarget) > 0.1f)
                    {
                        agent.SetDestination(currentTarget);
                    }

                    // Check if the worker has reached the free point
                    if (Vector3.Distance(transform.position, currentTarget) < 2.0f)
                    {
                        state = WorkerState.Delivering;
                        StartCoroutine(DeliverResource());
                    }
                }
                break;
        }
    }

    public void AssignResource(ResourceNode resource)
    {
        assignedResource = resource;
        float resourceRadius = resource.GetComponent<Collider>()?.bounds.extents.magnitude ?? 1.5f;
        Vector3 besidePoint = GetOrFindFreeBesidePoint(resource.transform.position, resourceRadius);
        agent.SetDestination(besidePoint);
        state = WorkerState.MovingToResource;
        isBusy = true;
    }

    public void GoTo(Vector3 position)
    {
        // If you have a NavMeshAgent, use it; otherwise, move directly
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(position);
        }
        else
        {
            // Simple fallback: teleport instantly (replace with your movement logic)
            transform.position = position;
        }
    }

    // In CollectResource, after collecting, go to beside point of townhall
    private System.Collections.IEnumerator CollectResource()
    {
        yield return new WaitForSeconds(collectTime);
        if (assignedResource && !assignedResource.IsDepleted())
        {
            assignedResource.Collect();
            isCarrying = true;
            float townhallRadius = townhall.GetComponent<Collider>()?.bounds.extents.magnitude ?? 2f;
            Vector3 besidePoint = GetOrFindFreeBesidePoint(townhall.transform.position, townhallRadius);
            agent.SetDestination(besidePoint);
            state = WorkerState.MovingToTownhall;
        }
        else
        {
            state = WorkerState.Idle;
            isBusy = false;
        }
    }

    // In DeliverResource, after delivering, go to beside point of resource node
    private System.Collections.IEnumerator DeliverResource()
    {
        yield return new WaitForSeconds(1f);
        if (assignedResource != null)
        {
            if (assignedResource.resourceType == ResourceType.Wood)
                ResourceManager.Instance.AddWood(carryAmount);
            else if (assignedResource.resourceType == ResourceType.Gold)
                ResourceManager.Instance.AddGold(carryAmount);
        }
        isCarrying = false;
        if (assignedResource && !assignedResource.IsDepleted())
        {
            float resourceRadius = assignedResource.GetComponent<Collider>()?.bounds.extents.magnitude ?? 1.5f;
            Vector3 besidePoint = GetOrFindFreeBesidePoint(assignedResource.transform.position, resourceRadius);
            agent.SetDestination(besidePoint);
            state = WorkerState.MovingToResource;
        }
        else
        {
            state = WorkerState.Idle;
            isBusy = false;
        }
    }

    // Call this when starting construction (from BuildingPlacer or similar):
    public void StartConstruction(GameObject building, Coroutine coroutine, int goldCost, int woodCost)
    {
        currentBuilding = building;
        constructionCoroutine = coroutine;
        lastGoldCost = goldCost;
        lastWoodCost = woodCost;
        state = WorkerState.Collecting; // Or a custom "Building" state if you have one
        isBusy = true;
    }

    public void SetConstructionProgressBar(GameObject bar)
    {
        constructionProgressBar = bar;
    }

    void OnMouseDown()
    {
        // If building, stop construction, refund 50%, and destroy building and progress bar
        if (state == WorkerState.Collecting && currentBuilding != null && constructionCoroutine != null)
        {
            StopCoroutine(constructionCoroutine);
            ResourceManager.Instance.AddGold(lastGoldCost / 2);
            ResourceManager.Instance.AddWood(lastWoodCost / 2);

            // Destroy the progress bar first
            if (constructionProgressBar != null)
            {
                Destroy(constructionProgressBar);
                constructionProgressBar = null;
            }

            // Signal cancellation to the coroutine
            if (currentBuilding != null)
            {
                var placer = FindFirstObjectByType<BuildingPlacer>();
                if (placer != null)
                {
                    var dict = placer.GetType().GetField("constructionCancelled", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(placer) as Dictionary<GameObject, bool>;
                    if (dict != null && dict.ContainsKey(currentBuilding))
                        dict[currentBuilding] = true;
                }
            }

            Destroy(currentBuilding);

            // Reset state
            currentBuilding = null;
            constructionCoroutine = null;
            lastGoldCost = 0;
            lastWoodCost = 0;
            state = WorkerState.Idle;
            isBusy = false;
            if (agent != null)
                agent.ResetPath();
            return;
        }

        // Otherwise, cancel any other task as before
        StopAllCoroutines();
        state = WorkerState.Idle;
        isBusy = false;
        assignedResource = null;
        isCarrying = false; // <-- Add this line to clear carried resource
        if (agent != null)
            agent.ResetPath();
    }
}