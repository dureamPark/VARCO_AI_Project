using System;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameTimeManage : MonoBehaviour
{
    public float currentTime { get; private set; } = 0.0f;

    public event Action<float> OnTimerEnded;
    
    private bool isRunning = false;

    private void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
        }
    }

    // 초기화 및 시작 
    public void BeginTimer()
    {
        currentTime = 0.0f;
        isRunning = true;
        Debug.Log("타이머 시작!");
    }

    // 일시 정지
    public void PauseTimer()
    {
        isRunning = false;
    }

    // 재개
    public void ResumeTimer()
    {
        isRunning = true;
    }

    // 종료
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"타이머 종료. 최종 기록: {currentTime:F2}초");
        OnTimerEnded?.Invoke(currentTime);
    }
}
