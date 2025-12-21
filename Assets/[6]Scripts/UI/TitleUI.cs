using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{

    public void StartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
