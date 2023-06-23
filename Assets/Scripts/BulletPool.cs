using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    public GameObject bulletPrefab;
    public int poolSize = 10;

    private Queue<GameObject> bullets;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bullets = new Queue<GameObject>();

        // Instantiate bullets and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bullets.Count == 0)
        {
            // If there are no bullets left in the pool, create a new one
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullets.Enqueue(bullet);
        }

        GameObject bulletToReturn = bullets.Dequeue();
        bulletToReturn.SetActive(true);
        return bulletToReturn;
    }


    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullets.Enqueue(bullet);
    }
}

