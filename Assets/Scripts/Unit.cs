using System.Collections;
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
    public float CurrentHealth { get; set; } = 50f;
    public int unitIndex;
    public int upgradeLevel;

    private AudioSource audioSource;
    public AudioClip shoot;
    public AudioClip dying;
    public float soundInterval;
    private bool playShootSound = true;
    private Animator animator;
    public bool IsBeingPlaced { get; set; } = false;

    public event System.Action OnUnitDied;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameObject.CompareTag("BlockedUnit") || IsBeingPlaced || animator.GetBool("isDead"))
        {
            return;
        }

        if (currentTarget == null || currentTarget.GetComponent<Enemy>().IsDead)
        {
            currentTarget = null;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (Time.time >= nextTargetSelectionTime || currentTarget == null)
        {
            SelectTarget();
            nextTargetSelectionTime = Time.time + targetSelectionCooldown;
        }

        if (currentTarget != null && Time.time >= nextAttackTime && Vector2.Distance(transform.position, currentTarget.transform.position) <= attackRange)
        {
            FlipTowardsEnemy(currentTarget.transform.position);
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
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemyCollider.gameObject;
                }
            }
        }

        currentTarget = closestEnemy;
    }

    private void FlipTowardsEnemy(Vector3 enemyPosition)
    {
        bool isEnemyToLeft = enemyPosition.x < transform.position.x;
        if (isEnemyToLeft != transform.localScale.x < 0)
        {
            Vector3 flippedScale = transform.localScale;
            flippedScale.x *= -1;
            transform.localScale = flippedScale;
        }
    }

    private void Attack(GameObject target)
    {
        if (!animator.GetBool("isAttacking"))
        {
            StartCoroutine(PerformAttack(target));
        }
    }

    private IEnumerator PerformAttack(GameObject target)
    {
        animator.SetBool("isAttacking", true);
        target.GetComponent<Enemy>().TakeDamage(attackDamage);

        if (playShootSound)
        {
            StartCoroutine(PlayShootingSound());
        }
        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("isAttacking", false);
    }

    private IEnumerator PlayShootingSound()
    {
        playShootSound = false;
        audioSource.clip = shoot;
        audioSource.Play();
        yield return new WaitForSeconds(soundInterval);
        playShootSound = true;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    public void Die()
    {
        StartCoroutine(PerformDeath());
    }

    private IEnumerator PerformDeath()
    {
        audioSource.clip = dying;
        audioSource.Play();
        animator.SetBool("isDead", true);
        gameObject.tag = "Untagged";
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(4);
        GameManager.instance.RemoveUnit(unitIndex);
        OnUnitDied?.Invoke();
        Destroy(gameObject);
    }

    public void UpgradeUnit()
    {
        upgradeLevel++;
        CurrentHealth += 5;

        if (attackCooldown > 0.2f)
        {
            attackCooldown -= 0.1f;
        }
        if (attackDamage < 50f)
        {
            attackDamage += 5;
        }
    }
}
