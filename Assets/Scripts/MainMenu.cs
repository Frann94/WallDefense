using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public AudioSource menuSound;

    public void StartGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        StartCoroutine(LoadSceneAfterSound("Map1"));
    }

    public void ExitGame()
    {
        StartCoroutine(LoadSceneAfterSound(null));
    }

    private IEnumerator LoadSceneAfterSound(string sceneName)
    {
        menuSound.Play();
        yield return new WaitForSeconds(menuSound.clip.length);
        if (sceneName != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Application.Quit();
        }
    }    
}
