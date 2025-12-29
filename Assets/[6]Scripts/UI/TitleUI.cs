using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject HTPPanel;

    private void Start()
    {
        HideHowToPlay();
    }
    public void StartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Stage2");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowHowToPlay()
    {
        HTPPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        HTPPanel.SetActive(false);
    }
}
