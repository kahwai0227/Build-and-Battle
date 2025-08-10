using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 30f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireCooldown = 0f;
    public bool isConstructed = false;

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
                transform.rotation = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, 90, 0); // Use -90 or 90 depending on your prefab;

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
                Vector3 dir = (target.transform.position - firePoint.position).normalized;
                projectile.SetDirection(dir);
            }
        }
    }
}