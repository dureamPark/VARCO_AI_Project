using System.Diagnostics;
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

    public void StartGame()
    {
        UnityEngine.Debug.Log("게임 시작 프로세스 가동...");

        // 1. 플레이어 초기화
        // player.Initialize();

        // 2. 시간 측정 시작 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.BeginTimer();
        }
        else
        {
            UnityEngine.Debug.Log("1GameTimeManager가 할당되지 않았습니다!");
        }

        // 3. UI 갱신 등...
    }

    public void GameClear()
    {
        UnityEngine.Debug.Log("게임 클리어!");

        // 1. 시간 측정 종료 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.StopTimer();
        }
        else
        {
            UnityEngine.Debug.Log("2GameTimeManager가 할당되지 않았습니다!");
        }

        // 2. 결과창 띄우기 등...
    }

    // 게임 오버 로직의 중심
    public void GameOver()
    {
        UnityEngine.Debug.Log("게임 오버!");

        // 1. 시간 측정 종료 명령
        if (Instance.timeManager != null)
        {
            Instance.timeManager.StopTimer();
        }
        else
        {
            UnityEngine.Debug.Log("3GameTimeManager가 할당되지 않았습니다!");
        }

        // 2. 결과창 띄우기 등...
    }
}
