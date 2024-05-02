using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.velocity = new Vector2(-Movespeeed, 0);
        AudioSource = GetComponent<AudioSource>();
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