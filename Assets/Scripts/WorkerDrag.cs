using UnityEngine;
using UnityEngine.AI;

public class WorkerDrag : MonoBehaviour
{
    public enum WorkerState { Idle, MovingToResource, Collecting, MovingToTownhall, Delivering }
    public WorkerState state = WorkerState.Idle;

    public ResourceNode assignedResource;
    public GameObject townhall;
    public float collectTime = 2f; // Time can stay the same
    public int carryAmount = 10;
    private NavMeshAgent agent;
    private bool isCarrying = false;
    public bool isBusy = false;

    // If you have movement speed:
    public float moveSpeed = 15f; // 3f * 5

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        townhall = GameObject.FindWithTag("Townhall");
    }

    void Update()
    {
        switch (state)
        {
            case WorkerState.MovingToResource:
                if (assignedResource && Vector3.Distance(transform.position, assignedResource.transform.position) < 7.5f) // 1.5f * 5
                {
                    state = WorkerState.Collecting;
                    StartCoroutine(CollectResource());
                }
                break;
            case WorkerState.MovingToTownhall:
                if (townhall && Vector3.Distance(transform.position, townhall.transform.position) < 7.5f) // 1.5f * 5
                {
                    state = WorkerState.Delivering;
                    StartCoroutine(DeliverResource());
                }
                break;
        }
    }

    public void AssignResource(ResourceNode resource)
    {
        assignedResource = resource;
        agent.SetDestination(resource.transform.position);
        state = WorkerState.MovingToResource;
    }

    public void GoTo(Vector3 target)
    {
        GetComponent<NavMeshAgent>().SetDestination(target);
    }

    private System.Collections.IEnumerator CollectResource()
    {
        yield return new WaitForSeconds(collectTime);
        if (assignedResource)
        {
            assignedResource.Collect(carryAmount);
            isCarrying = true;
            agent.SetDestination(townhall.transform.position);
            state = WorkerState.MovingToTownhall;
        }
    }

    private System.Collections.IEnumerator DeliverResource()
    {
        yield return new WaitForSeconds(1f);
        ResourceManager.Instance.AddGold(carryAmount); // Or AddWood, depending on resource type
        isCarrying = false;
        if (assignedResource)
        {
            agent.SetDestination(assignedResource.transform.position);
            state = WorkerState.MovingToResource;
        }
        else
        {
            state = WorkerState.Idle;
        }
    }

    private void UpdateBarHeight(Collider col, GameObject barObj)
    {
        float barHeight = col != null ? col.bounds.extents.y * 2.5f : 10f; // 2.5f gives more margin for larger objects
        barObj.transform.localPosition = new Vector3(0, barHeight, 0);
    }
}