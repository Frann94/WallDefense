using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    private GameObject target;
    private bool isFiring;

    public void FireAtTarget(GameObject target)
    {
        this.target = target;
        isFiring = true;
    }

    private void Update()
    {
        if (isFiring && target != null)
        {
            // Move the bullet towards the target
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // If the bullet has reached the target
            if (transform.position == target.transform.position)
            {
                isFiring = false;
                BulletPool.Instance.ReturnBullet(gameObject);
            }
        }
    }
}

