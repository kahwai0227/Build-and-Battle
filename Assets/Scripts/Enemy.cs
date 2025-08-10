using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public int health = 10;
    private Transform target;

    void Start()
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

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Destroy(gameObject);
    }
}