using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConfig : MonoBehaviour
{
    private void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == 1) {
                SceneManager.LoadScene(sceneIndex - 1);
            }
        }
    }
}
