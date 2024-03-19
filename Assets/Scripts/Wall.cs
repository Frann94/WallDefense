using UnityEngine;
using UnityEngine.SceneManagement;

public class Wall : MonoBehaviour
{
    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; } = 100f;

    private void Update()
    {
        if (CurrentHealth <= 0)
        {
            Lose();
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Lose();
        }
    }

    private void Lose()
    {
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.ActivateGameOverPanel();
        }
    }
}
