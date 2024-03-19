using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Movespeeed { get; set; } = 5f;
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    public float Damage { get; set; } = 5f;
    public float Range { get; set; } = 1f;
    public float FireRate { get; set; } = 1f;
    public AudioSource AudioSource { get; set; }
    public bool IsDead { get => _isDead; protected set => _isDead = value; }

    protected Rigidbody2D Body;
    private Animator _animator;
    private bool _isDead;
    private bool _canAttack = true;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected void Move()
    {
        if (!_isDead) {
            GameObject target = FindNearestToAttack();
            if ((target.transform.position - transform.position).magnitude > Range)
            {
                if (target.tag == "Wall" && transform.position.x > target.transform.position.x)
                {
                    Body.velocity = new Vector2(-Movespeeed, 0f);
                }
                else if (target.tag == "Player")
                {
                    Body.velocity = (FindNearestToAttack().transform.position - transform.position).normalized * Movespeeed;
                }
                else
                {
                    Body.velocity = new Vector2(0, 0);
                    Attack(target);
                }
            }
            else
            {
                Body.velocity = new Vector2(0, 0);
                Attack(target);
            }
        }
    }

    protected GameObject FindNearestToAttack()
    {
        GameObject[] attackable = GameObject.FindGameObjectsWithTag("Player");
        GameObject target = GameObject.FindGameObjectWithTag("Wall");
        foreach (GameObject a in attackable)
        {
            if ((transform.position - a.transform.position).magnitude < (transform.position - target.transform.position).magnitude)
            {
                target = a;
            }
        }
        return target;
    }

    private void Attack(GameObject target)
    {
        if (_canAttack && !IsDead)
        {
            StartCoroutine(PerformAttack(target));
        }
    }

    private IEnumerator PerformAttack(GameObject target)
    {
        _canAttack = false;
        _animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1.8f);
        DealDamage(target);
        _animator.SetBool("isAttacking", false);
        _canAttack = true;
    }

    private void DealDamage(GameObject target)
    {
        var unitTarget = target.GetComponent<Unit>();
        if (unitTarget != null)
        {
            unitTarget.TakeDamage(Damage);
        }

        var wallTarget = target.GetComponent<Wall>();
        if (wallTarget != null)
        {
            wallTarget.TakeDamage(Damage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead)
        {
            return;
        }

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        if (IsDead)
        {
            return;
        }

        IncrementPlayerPoints();
        StartCoroutine(PlayDyingSoundAndDestroy());
    }

    private void IncrementPlayerPoints()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.IncrementPoints(10); 
        }
    }

    private IEnumerator PlayDyingSoundAndDestroy()
    {
        IsDead = true;
        _animator.SetBool("isDead", true);
        Body.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        AudioSource.Play();
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
