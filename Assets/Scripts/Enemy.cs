using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Movespeeed { get; set; } = 5f;
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    public float Damage { get; set; } = 5f;
    public float Range { get; set; } = 1.25f;
    public float FireRate { get; set; } = 1f;
    public AudioSource AudioSource { get; set; }
    public bool IsDead { get => _isDead; protected set => _isDead = value; }

    protected Rigidbody2D Body;
    private Animator _animator;
    private GameObject _target;
    private bool _isDead;
    private bool _canAttack = true;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected void Move()
    {
        if (!_isDead)
        {
            if (_target == null)
            {
                _target = GameObject.FindGameObjectWithTag("Wall");
            }
            GameObject targetUnit = FindNearestUnit();
            if (targetUnit != null)
            {
                _target = targetUnit;
            }
            float distanceToTarget = Mathf.Abs(transform.position.x - _target.transform.position.x);

            if (distanceToTarget <= Range)
            {
                Body.velocity = new Vector2(0, 0);
                Attack(_target);
            }
            else
            {
                Body.velocity = (_target.transform.position - transform.position).normalized * Movespeeed;
            }
        }
    }

    protected GameObject FindNearestUnit()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Player");
        GameObject targetUnit = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject unit in units)
        {
            float distance = Mathf.Abs(transform.position.magnitude - unit.transform.position.magnitude);
            if (distance <= Range * 2 && distance < shortestDistance)
            {
                targetUnit = unit;
                shortestDistance = distance;
            }
        }

        return targetUnit;
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
            if (unitTarget.CurrentHealth <= 0)
            {
                _target = null;
            }
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
        ShowFloatingText(5);
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

    private void ShowFloatingText(int points)
    {
        GameObject pointsText = GameManager.instance.pointsText;
        if (pointsText != null)
        {
            GameObject floatingText = Instantiate(pointsText, transform.position, Quaternion.identity);
            floatingText.GetComponent<TextMesh>().text = "+" + points.ToString();
            floatingText.transform.position += new Vector3(0f, 1.5f, 0f);
            StartCoroutine(FadeOutFloatingText(floatingText));
        }
    }

    private IEnumerator FadeOutFloatingText(GameObject floatingText)
    {
        TextMesh textMesh = floatingText.GetComponent<TextMesh>();
        Vector3 startPosition = floatingText.transform.position;
        Color startColor = textMesh.color;
        Vector3 targetPosition = startPosition + Vector3.up * 2f;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            floatingText.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 3f);
            textMesh.color = Color.Lerp(startColor, targetColor, elapsedTime / 3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(floatingText);
    }
}
