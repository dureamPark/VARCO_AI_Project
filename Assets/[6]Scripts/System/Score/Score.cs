using System.Diagnostics;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int damagePoint = 10; // 적에게 입힌 데미지 당 점수

    [SerializeField]
    private int timeBonusPoint = 0; // 스테이지 클리어 시간 보너스 점수

    private GameTimeManager timeManager;

    public int scorePoint => score;

    // 점수 획득하는 경우
    /*  1. 적에게 입힌 데미지 : 적에게 입힌 데미지를 포인트에 합산한다.
        2. 각 스테이지를 클리어한 시간 : 각 스테이지 클리어한 시간을 기준과 비교해 등급별로 합산한다.
        3. 적의 공격을 아슬아슬하게 피했을 때 : 플레이어의 피격 판정 주위에 데미지 포인트(DP)의 범위를 설정해 적의
            공격이 안으로 들어올 때 마다 점수 합산  
    */

    private void Awake()
    {
        if (timeManager == null)
        {
            timeManager = GetComponent<GameTimeManager>();
        }
    }

    private void Start()
    {
        if (timeManager != null)
        {
            UnityEngine.Debug.Log("score : GameTimeManager가 할당되었습니다.");
            timeManager.OnTimerEnded += HandleTimerEnded;
        }
    }

    private void OnDestroy()
    {
        //구독 해지: 오브젝트가 사라질 때 연결 끊기
        if (timeManager != null)
        {
            UnityEngine.Debug.Log("score : GameTimeManager 구독 해지.");
            timeManager.OnTimerEnded -= HandleTimerEnded;
        }
    }

    private void HandleTimerEnded(float finalTime)
    {        
        AddTimeBonusScore(finalTime);
        UnityEngine.Debug.Log($"점수 갱신 완료! 현재 점수: {score}");
        OnDestroy();
    }

    public void AddDamageScore(int damage)
    {
        score += damage * damagePoint;
    }

    public void AddTimeBonusScore(float totalTime)
    {
        UnityEngine.Debug.Log($"add bonus score 총 클리어 시간: {totalTime:F2}초");
        // 시간에 따라 보너스 점수 부여
        if (totalTime >= 60)
        {
            timeBonusPoint = 1000;
        }
        else if (totalTime >= 30)
        {
            timeBonusPoint = 3000;
        }
        else if (totalTime >= 10)
        {
            timeBonusPoint = 5000;
        }
        else
        {
            timeBonusPoint = 123;
        }

        score += timeBonusPoint;

        UnityEngine.Debug.Log($"시간 점수: {timeBonusPoint}");
    }
}
