using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Movespeeed { get; set; } = 5f;
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    public float Damage { get; set; } = 5f;
    public float Range { get; set; } = 1f;

    protected Rigidbody2D body;
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

    protected void Move()
    {
        GameObject target = FindNearestToAttack();
        if ((target.transform.position - transform.position).magnitude > Range)
        {
            if (target.tag == "Wall" && transform.position.x > target.transform.position.x)
            {
                body.velocity = new Vector2(-Movespeeed, 0f);
            }
            else if (target.tag == "Player")
            {
                body.velocity = (FindNearestToAttack().transform.position - transform.position).normalized * Movespeeed;
            }
            else
            {
                body.velocity = new Vector2(0, 0);
            }
        }
        else
        {
            body.velocity = new Vector2(0, 0);
        }
    }
}
