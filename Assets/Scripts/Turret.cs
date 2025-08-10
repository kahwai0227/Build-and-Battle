using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        Enemy target = FindNearestEnemy();
        if (target != null && fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / fireRate;
        }
    }

    Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
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
            proj.GetComponent<Rigidbody>().linearVelocity = (target.transform.position - firePoint.position).normalized * 10f;
            // Add logic for projectile damage on hit
        }
    }
}