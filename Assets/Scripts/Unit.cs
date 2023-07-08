using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float attackRange = 5f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.1f;
    public float targetSelectionCooldown = 5f;

    private GameObject currentTarget;
    private float nextAttackTime;
    private float nextTargetSelectionTime;
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 5f;
    public bool IsPlaced { get; set; } = false;

    private void Update()
    {
        if (IsPlaced)
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
            if (CurrentHealth <= 0)
            {
                Die();
            }
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
        // Damage the target
        target.GetComponent<Enemy>().TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage) {
        CurrentHealth -= damage;
        Debug.Log(CurrentHealth);
    }

    protected void Die()
    {
        GameManager.instance.RemoveUnit(this);
        Destroy(gameObject);
    }
}

