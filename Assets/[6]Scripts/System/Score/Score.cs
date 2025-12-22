using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int damagePoint = 10; // 적에게 입힌 데미지 당 점수

    [SerializeField]
    private int timeBonusPoint = 0; // 스테이지 클리어 시간 보너스 점수

    public int scorePoint => score;

    // 점수 획득하는 경우
    /*  1. 적에게 입힌 데미지 : 적에게 입힌 데미지를 포인트에 합산한다.
        2. 각 스테이지를 클리어한 시간 : 각 스테이지 클리어한 시간을 기준과 비교해 등급별로 합산한다.
        3. 적의 공격을 아슬아슬하게 피했을 때 : 플레이어의 피격 판정 주위에 데미지 포인트(DP)의 범위를 설정해 적의
            공격이 안으로 들어올 때 마다 점수 합산  
    */
    public void AddDamageScore(int damage)
    {
        score += damage * damagePoint;
    }

    public void AddTimeBonusScore(int remainTime)
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
