using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void StartApplication() {
        AudioManager.Instance.PlayButtonClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitApplication() {
        AudioManager.Instance.PlayButtonBackClickSound();
        Application.Quit();
    }
}
