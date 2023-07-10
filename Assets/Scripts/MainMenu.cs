using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public AudioSource menuSound;

    public void StartGame()
    {
        StartCoroutine(PlaySoundAndLoadScene("Map1"));
    }

    public void ExitGame()
    {
        // Sound for Exit game is usually different
        StartCoroutine(PlaySoundAndExit());
    }

    private IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        menuSound.Play();
        yield return new WaitForSeconds(menuSound.clip.length);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator PlaySoundAndExit()
    {
        menuSound.Play();
        yield return new WaitForSeconds(menuSound.clip.length);
        Application.Quit();
    }
}