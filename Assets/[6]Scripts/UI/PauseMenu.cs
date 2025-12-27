using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    // ▼▼▼ 이 부분 추가! ▼▼▼
    void Start()
    {
        // 게임 시작하면 무조건 패널을 숨기고 시작함
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume(); // ESC 다시 누르면 꺼짐
            }
            else
            {
                Pause(); // ESC 누르면 켜짐
            }
        }
    }

    public void Resume()
    {
        AudioEvents.TriggerPlaySFX("ButtonClick");
        pausePanel.SetActive(false); // 버튼으로 눌러도 이게 실행되어 꺼짐
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        AudioEvents.TriggerPlaySFX("ButtonClick");
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ExitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}