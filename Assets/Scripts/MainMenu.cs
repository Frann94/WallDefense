using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene("Menu&UI");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
