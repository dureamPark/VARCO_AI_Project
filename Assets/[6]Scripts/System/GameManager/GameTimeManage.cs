using System.Diagnostics;
using UnityEngine;

public class GameTimeManage : MonoBehaviour
{
    [SerializeField]
    private float gameTime = 0.0f; // 게임 진행 시간

    [SerializeField]
    private bool isGameStart = false;

    public float GameTime => gameTime;

    private void Update()
    {
        if (isGameStart)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void StartGameTime()
    {
        isGameStart = true;
        gameTime = 0.0f;
        
    }
    public void StopGameTime()
    {
        isGameStart = false;
        //Debug.Log("게임 시간 종료: " + gameTime + "초");
        gameTime = 0.0f;
    }

}
