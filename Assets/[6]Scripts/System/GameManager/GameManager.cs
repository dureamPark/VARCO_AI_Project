using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static GameManager Instance;

    [Header("Managers")]
    [SerializeField]
    private GameTimeManager timeManager;
    [SerializeField]
    private Score score;

    // 상태 변수
    private bool isPaused = false;      // 현재 일시정지 상태인가?
    private bool isGameActive = false;  // 게임이 플레이 중인가?

    public GameTimeManager TimeManager => timeManager;
    public Score scoreManager => score;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    private void Start()
    {
        StartGame(); 
    }

    private void Update()
    {
        // 일시정지 토글
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // 게임 중일 때만 작동
            if (isGameActive)
            {
                TogglePause();
            }
        }
    }

    public void StartGame()
    {
        UnityEngine.Debug.Log("게임 시작 프로세스 가동...");
        PlayerPrefs.DeleteKey("SavedStage"); // 이전에 저장된 스테이지 삭제

        // 게임 상태 활성화
        isGameActive = true;
        isPaused = false;

        // 시간 정상화 
        Time.timeScale = 1f;

        // 시간 측정 시작 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.BeginTimer();
        }
        else
        {
            UnityEngine.Debug.Log("1GameTimeManager가 할당되지 않았습니다!");
        }

        // UI 갱신 등...
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 시간 정지
        UnityEngine.Debug.Log("일시정지");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 시간 정상화
        UnityEngine.Debug.Log("게임 재개");
    }

    public void GameClear()
    {
        isGameActive = false;
        UnityEngine.Debug.Log("게임 클리어!");
        PlayerPrefs.DeleteKey("SavedStage"); // 저장된 스테이지 삭제

        // 시간 측정 종료 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.StopTimer();
        }
        else
        {
            UnityEngine.Debug.Log("2GameTimeManager가 할당되지 않았습니다!");
        }

        // 결과창 띄우기 등...
    }

    // 게임 오버 로직의 중심
    public void GameOver()
    {
        isGameActive = false;
        UnityEngine.Debug.Log("게임 오버!");

        if (StageManager.Instance != null)
        {
            PlayerPrefs.SetInt("SavedStage", StageManager.Instance.currentStage);
            UnityEngine.Debug.Log($"현재 스테이지 저장 : {StageManager.Instance.currentStage}");
            PlayerPrefs.Save(); // 저장
        }

        // 시간 측정 종료 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.StopTimer();
            //RetryGame();
        }
        else
        {
            UnityEngine.Debug.Log("3GameTimeManager가 할당되지 않았습니다!");
        }

        // 결과창 띄우기 등...
    }

    public void RetryGame()
    {
        // 멈췄던 시간 다시 흐르게 설정
        Time.timeScale = 1f;

        // 현재 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
