using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed = 100f;

    public override void Start()
    {
        base.Start();
        // Ranged enemies have a larger attack range
        attackRange = 10f;
    }

    void Update()
    {
        // Check for nearby buildings
        DetectNearbyBuilding();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // Stop moving if within attack range
            if (distance > attackRange)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                // Attack the target if within range
                if (Time.time > lastAttackTime + attackInterval)
                {
                    AttackTarget();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            // Spawn a projectile and shoot it toward the target
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(target.transform); // Use the projectile script to target the building
            }
        }
    }
}