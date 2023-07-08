using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField]
    private GameObject target;
    private bool isFiring;
    private Vector3 targetPosition;

    public void FireAtTarget(GameObject target)
    {
        this.target = target;
        isFiring = true;
    }

    private void Update()
    {
        // Move the bullet towards the target
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (isFiring && target != null)
        {
            targetPosition = target.transform.position;            
        }
        // If the bullet has reached the target
        if (transform.position == targetPosition)
        {
            isFiring = false;
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}

