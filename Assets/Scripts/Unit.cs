using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackInterval = 1f;
    private float lastAttackTime;
    private NavMeshAgent agent;
    public bool isIdle = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        if (agent != null)
        {
            Debug.Log($"Unit moving to: {destination}");
            agent.SetDestination(destination);
            isIdle = false;
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is missing!");
        }
    }

    public void Attack(Enemy enemy)
    {
        if (enemy != null && Time.time > lastAttackTime + attackInterval)
        {
            enemy.TakeDamage((int)attackDamage);
            lastAttackTime = Time.time;
            isIdle = false;
        }

        // Move closer to the enemy if not in range
        if (agent != null && Vector3.Distance(transform.position, enemy.transform.position) > agent.stoppingDistance)
        {
            agent.SetDestination(enemy.transform.position);
        }
    }

    void Update()
    {
        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isIdle = true;
        }
    }
}