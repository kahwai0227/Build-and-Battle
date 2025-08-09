using UnityEngine;
using UnityEngine.AI;

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
                if (assignedResource && Vector3.Distance(transform.position, assignedResource.transform.position) < 1.5f)
                {
                    state = WorkerState.Collecting;
                    StartCoroutine(CollectResource());
                }
                break;
            case WorkerState.MovingToTownhall:
                if (townhall && Vector3.Distance(transform.position, townhall.transform.position) < 1.5f)
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
}