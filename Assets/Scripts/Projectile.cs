using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public int damage = 5;
    public float lifetime = 15f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        // Rotate the capsule so it faces the direction of travel and lays flat
        if (direction != Vector3.zero)
        {
            // Look in the direction, then rotate 90 degrees around local X to lay flat
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(90f, 0f, 0f, Space.Self);
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}