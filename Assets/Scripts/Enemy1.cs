using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(-Movespeeed, 0);
        fireCount = FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        fireCount -= Time.deltaTime;
        if (CurrentHealth <= 0) {
            Die();
        }
    }
}