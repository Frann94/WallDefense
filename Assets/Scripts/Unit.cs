using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Sprite unitIcon;
    public int cost;
    public float cooldown;

    public LayerMask enemyLayer;
    public float attackRange = 5f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.1f;
    public float targetSelectionCooldown = 5f;

    private GameObject currentTarget;
    private float nextAttackTime;
    private float nextTargetSelectionTime;
    public float MaxHealth { get; set; } = 1000f;
    public float CurrentHealth { get; set; } = 5f;
    public event System.Action OnUnitDied;
    public int unitIndex;
    public int upgradeLevel;
    private AudioSource audioSource;
    public AudioClip shoot;
    public AudioClip dying;
    public float soundInterval;
    private bool playShootSound = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
            // Only select a new target if the cooldown has elapsed and the current target is null or out of range
            if (Time.time >= nextTargetSelectionTime || currentTarget == null)
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
        if (playShootSound)
        {
            StartCoroutine(PlayShootingSound());
        }
    }

    private IEnumerator PlayShootingSound()
    {
        playShootSound = false;
        audioSource.clip = shoot;
        audioSource.Play();
        yield return new WaitForSeconds(soundInterval); // Wait for 1 second
        playShootSound = true;
    }

    private IEnumerator PlayDyingSound()
    {
        audioSource.clip = dying;
        audioSource.Play();
        yield return new WaitForSeconds(2);
    }

    public void TakeDamage(float damage) {
        CurrentHealth -= damage;
    }

    public void Die()
    {
        StartCoroutine(PlayDyingSound());
        GameManager.instance.RemoveUnit(unitIndex);
        OnUnitDied?.Invoke();
        Destroy(gameObject);
    }
}

