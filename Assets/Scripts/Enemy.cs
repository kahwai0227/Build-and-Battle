using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    public int health = 10;
    private Transform target;
    public EnemySpawner spawner; // Add this at the top

    public virtual void Start()
    {
        GameObject townhall = GameObject.FindWithTag("Townhall");
        if (townhall != null)
            target = townhall.transform;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TownhallHealth townhall = other.GetComponent<TownhallHealth>();
        if (townhall != null)
        {
            townhall.TakeDamage(10);
            if (spawner != null)
                spawner.OnEnemyDefeated();
            Destroy(gameObject);
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