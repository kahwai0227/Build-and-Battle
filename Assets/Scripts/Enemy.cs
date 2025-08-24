using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    public int health = 10;
    public float attackDamage = 10f;
    public float attackInterval = 2f; // Time between attacks
    protected float lastAttackTime;
    protected Transform target;
    public EnemySpawner spawner;

    public float detectionRadius = 20f; // Radius to detect nearby buildings or units
    public float attackRange = 3f; // Distance to stop and attack

    private bool isAttacking = false;
    private NavMeshAgent agent;

    public virtual void Start()
    {
        // Initialize NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }

        // Set the initial target to the townhall
        GameObject townhall = GameObject.FindWithTag("Townhall");
        if (townhall != null)
            target = townhall.transform;
    }

    void Update()
    {
        // Check for nearby units or buildings
        DetectNearbyTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // Move towards the target if not within attack range
            if (distance > attackRange)
            {
                if (agent != null)
                {
                    agent.SetDestination(target.position);
                }
                isAttacking = false;
            }
            else
            {
                // Stop moving and attack the target
                if (agent != null)
                {
                    agent.ResetPath();
                }
                isAttacking = true;
            }
        }

        // Attack the target if within range
        if (isAttacking && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange && Time.time > lastAttackTime + attackInterval)
            {
                Unit unit = target.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.TakeDamage((int)attackDamage);
                    lastAttackTime = Time.time;
                }
                else
                {
                    Building building = target.GetComponent<Building>();
                    if (building != null)
                    {
                        building.TakeDamage(attackDamage);
                        lastAttackTime = Time.time;
                    }
                }
            }
        }
    }

    protected void DetectNearbyTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Unit", "Building"));
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider collider in colliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            Building building = collider.GetComponent<Building>();

            if (unit != null && unit.health > 0)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = unit.transform;
                }
            }
            else if (building != null && building.health > 0)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = building.transform;
                }
            }
        }

        // If a nearby target is found, target the closest one
        if (closestTarget != null)
        {
            target = closestTarget;
            isAttacking = false; // Reset attacking state
        }
        else
        {
            // If no nearby target is found, target the townhall
            GameObject townhall = GameObject.FindWithTag("Townhall");
            if (townhall != null)
            {
                target = townhall.transform;
                isAttacking = false; // Reset attacking state
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (spawner != null)
                spawner.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }
}