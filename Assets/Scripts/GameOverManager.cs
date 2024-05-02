using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI totalPointsText;
    private LevelManager _levelManager;

    void Start()
    {
        gameOverPanel.SetActive(false);
        _levelManager = FindObjectOfType<LevelManager>();
    }

    public void ActivateGameOverPanel()
    {
        totalTimeText.text = $"Time Survived: {_levelManager.GetFormattedTime()}";
        totalPointsText.text = $"Points Earned: {_levelManager.TotalPointsEarned}";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
