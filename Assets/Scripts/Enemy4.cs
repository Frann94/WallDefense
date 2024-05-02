using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : Enemy
{
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.velocity = new Vector2(-Movespeeed, 0);
        AudioSource = GetComponent<AudioSource>();
        CurrentHealth = 1000f;
        Damage = 50f;
        Movespeeed = 2f;
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        Move();
    }
}