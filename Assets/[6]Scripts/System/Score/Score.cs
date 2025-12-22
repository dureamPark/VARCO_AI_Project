using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int damagePoint = 10; // 적에게 입힌 데미지 당 점수

    [SerializeField]
    private int timeBonusPoint = 0; // 스테이지 클리어 시간 보너스 점수

    private GameTimeManage timeManage;

    public int scorePoint => score;

    // 점수 획득하는 경우
    /*  1. 적에게 입힌 데미지 : 적에게 입힌 데미지를 포인트에 합산한다.
        2. 각 스테이지를 클리어한 시간 : 각 스테이지 클리어한 시간을 기준과 비교해 등급별로 합산한다.
        3. 적의 공격을 아슬아슬하게 피했을 때 : 플레이어의 피격 판정 주위에 데미지 포인트(DP)의 범위를 설정해 적의
            공격이 안으로 들어올 때 마다 점수 합산  
    */

    private void Start()
    {
        if (timeManage != null)
        {
            timeManage.OnTimerEnded += HandleTimerEnded;
        }
    }

    private void OnDestroy()
    {
        //구독 해지: 오브젝트가 사라질 때 연결 끊기 (필수!)
        if (timeManage != null)
        {
            timeManage.OnTimerEnded -= HandleTimerEnded;
        }
    }

    private void HandleTimerEnded(float finalTime)
    {
        // float 시간을 int로 변환해서 기존 로직 실행
        // 주의: 남은 시간(RemainTime) 계산이 필요하다면 여기서 처리해야 함
        // 예: int remainTime = 100 - (int)finalTime;
        
        // 일단은 흐른 시간을 그대로 넘깁니다.
        AddTimeBonusScore(finalTime);
        Debug.Log($"점수 갱신 완료! 현재 점수: {score}");
    }

    public void AddDamageScore(int damage)
    {
        score += damage * damagePoint;
    }

    public void AddTimeBonusScore(float remainTime)
    {
        // 시간에 따라 보너스 점수 부여
        if (remainTime >= 60)
        {
            timeBonusPoint = 5000;
        }
        else if (remainTime >= 30)
        {
            timeBonusPoint = 3000;
        }
        else if (remainTime >= 10)
        {
            timeBonusPoint = 1000;
        }
        else
        {
            timeBonusPoint = 100;
        }

        score += timeBonusPoint;
    }
}
