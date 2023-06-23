using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float attackRange = 5f;
    public float attackDamage = 1f;
    public float attackCooldown = 1f;
    public float targetSelectionCooldown = 5f;

    private GameObject currentTarget;
    private float nextAttackTime;
    private float nextTargetSelectionTime;

    private void Update()
    {
        // Only select a new target if the cooldown has elapsed and the current target is null or out of range
        if (Time.time >= nextTargetSelectionTime)
        {
            SelectTarget();
            nextTargetSelectionTime = Time.time + targetSelectionCooldown;
        }

        // Only attack if there is a target and the attack cooldown has elapsed
        if (currentTarget != null && Time.time >= nextAttackTime && Vector2.Distance(transform.position, currentTarget.transform.position) <= attackRange)
        {
            Attack(currentTarget);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void SelectTarget()
    {
        // Find all enemies within attack range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            // Select the closest enemy
            GameObject closestEnemy = enemiesInRange[0].gameObject;

            for (int i = 1; i < enemiesInRange.Length; i++)
            {
                if (Vector2.Distance(transform.position, enemiesInRange[i].transform.position) < Vector2.Distance(transform.position, closestEnemy.transform.position))
                {
                    closestEnemy = enemiesInRange[i].gameObject;
                }
            }

            // If there's no current target, or if the closest enemy is closer than the current target, switch targets
            if (currentTarget == null || Vector2.Distance(transform.position, closestEnemy.transform.position) < Vector2.Distance(transform.position, currentTarget.transform.position))
            {
                currentTarget = closestEnemy;
            }
        }
    }


    private void Attack(GameObject target)
    {
        // Get a bullet from the bullet pool
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = transform.position;
        bullet.SetActive(true);

        // Fire the bullet at the target
        bullet.GetComponent<Bullet>().FireAtTarget(target);

        // Damage the target
        target.GetComponent<Enemy>().TakeDamage(attackDamage);
    }

}

