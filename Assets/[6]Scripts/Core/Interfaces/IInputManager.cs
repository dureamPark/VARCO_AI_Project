using UnityEngine;

public interface IInputManager
{
    Vector2 GetMovementInput(); // WASD 이동

    // 공격 (Z키 토글)
    bool GetAttackDown();

    // 스킬 1: 유도탄 모드 변경 (X키)
    bool GetFlowStyleDown();

    // 스킬 2: 차원 방벽 (C키) 
    bool GetBarrierKey();

    // 스킬 3: 필살기 (Ctrl키)
    bool GetOverWriteDown();

    // 대화 (Space)
    bool GetInteractDown();
}