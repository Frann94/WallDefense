using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.velocity = new Vector2(-Movespeeed, 0);
        AudioSource = GetComponent<AudioSource>();
        CurrentHealth = 75f;
        Movespeeed = 10f;
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