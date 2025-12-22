using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static GameManager Instance;

    [Header("Managers")]
    [SerializeField]
    private GameTimeManage timeManage;

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
        Debug.Log("게임 시작 프로세스 가동...");

        // 1. 플레이어 초기화
        // player.Initialize();

        // 2. 시간 측정 시작 명령
        if (timeManage != null)
        {
            timeManage.BeginTimer();
        }

        // 3. UI 갱신 등...
    }

    // 게임 오버 로직의 중심
    public void GameOver()
    {
        Debug.Log("게임 오버!");

        // 1. 시간 측정 종료 명령
        if (timeManage != null)
        {
            timeManage.StopTimer();
        }

        // 2. 결과창 띄우기 등...
    }
}
