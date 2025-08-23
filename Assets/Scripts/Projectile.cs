using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public int damage = 5;
    public float lifetime = 15f;

    private Vector3 direction;
    private Transform target;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        Debug.Log($"Projectile target set to: {target.name}");
    }

    void Start()
    {
        // Rotate the capsule so it faces the direction of travel and lays flat
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(90f, 0f, 0f, Space.Self);
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            // Move toward the target
            direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Rotate to face the target
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(90f, 0f, 0f, Space.Self);
        }
        else
        {
            // Move in the assigned direction if no target is set
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile collided with: {other.name}");

        // Check if the target is an Enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && other.transform == target)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Check if the target is a Building
        Building building = other.GetComponent<Building>();
        if (building != null && other.transform == target)
        {
            building.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}