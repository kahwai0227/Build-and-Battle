using UnityEngine;

public class RangedUnit : Unit
{
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed = 20f;

    public override void Attack(Enemy enemy)
    {
        if (enemy != null && Time.time > lastAttackTime + attackInterval)
        {
            // Spawn a projectile and shoot it toward the enemy
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(enemy.transform);
                projectileScript.damage = (int)attackDamage;
            }
            lastAttackTime = Time.time;
            isIdle = false;
        }

        // Move closer to the enemy if not in range
        if (agent != null && Vector3.Distance(transform.position, enemy.transform.position) > agent.stoppingDistance)
        {
            agent.SetDestination(enemy.transform.position);
        }
    }
}