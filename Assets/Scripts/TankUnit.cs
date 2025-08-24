using UnityEngine;

public class TankUnit : Unit
{
    public override void Attack(Enemy enemy)
    {
        if (enemy != null && Time.time > lastAttackTime + attackInterval)
        {
            enemy.TakeDamage((int)attackDamage);
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