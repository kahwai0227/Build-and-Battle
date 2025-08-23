using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    public int health = 10;
    public float attackDamage = 10f;
    public float attackInterval = 2f; // Time between attacks
    protected float lastAttackTime;
    protected Transform target;
    public EnemySpawner spawner;

    public float detectionRadius = 20f; // Radius to detect nearby buildings
    public float attackRange = 3f; // Distance to stop and attack

    private bool isAttacking = false;

    public virtual void Start()
    {
        GameObject townhall = GameObject.FindWithTag("Townhall");
        if (townhall != null)
            target = townhall.transform;
    }

    void Update()
    {
        // Check for nearby buildings
        DetectNearbyBuilding();

        if (target != null && !isAttacking)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            // Stop moving if within attack range
            if (distance > attackRange)
            {
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                isAttacking = true;
            }
        }

        // Attack the target if within range
        if (isAttacking && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange && Time.time > lastAttackTime + attackInterval)
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

    protected void DetectNearbyBuilding()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Building"));
        float closestDistance = Mathf.Infinity;
        Transform closestBuilding = null;

        foreach (Collider collider in colliders)
        {
            Building building = collider.GetComponent<Building>();
            if (building != null && building.health > 0)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = building.transform;
                }
            }
        }

        // If a nearby building is found, target the closest one
        if (closestBuilding != null)
        {
            target = closestBuilding;
            isAttacking = false; // Reset attacking state
        }
        else
        {
            // If no nearby building is found, target the townhall
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