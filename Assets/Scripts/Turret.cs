using UnityEngine;

public class Turret : Building
{
    public float range = 30f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float fireCooldown = 0f;

    void Update()
    {
        if (!isConstructed)
            return;

        fireCooldown -= Time.deltaTime;
        Enemy target = FindNearestEnemy();
        if (target != null)
        {
            // Rotate turret to face the target (only on Y axis)
            Vector3 lookDir = target.transform.position - transform.position;
            lookDir.y = 0; // Keep turret upright
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, 90, 0); // Adjust rotation based on prefab orientation

            if (fireCooldown <= 0f)
            {
                Shoot(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDist = range;
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void Shoot(Enemy target)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = proj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetTarget(target.transform); // Use the projectile script to target the enemy
            }
        }
    }

    protected override void DestroyBuilding()
    {
        Debug.Log("Turret has been destroyed!");
        base.DestroyBuilding();
    }
}