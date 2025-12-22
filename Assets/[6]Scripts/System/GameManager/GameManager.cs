using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameTimeManage timeManage; // 인스펙터에서 연결

    void StartActualGame()
    {
        timeManage.StartGameTime(); 
    }
}
