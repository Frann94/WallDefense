using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentHealth <= 0)
        {
            Lose();
        }
    }

    public void TakeDamage(float damage) {
        CurrentHealth -= damage;
    }

    private void Lose() {
        Time.timeScale = 0;
    }
}
