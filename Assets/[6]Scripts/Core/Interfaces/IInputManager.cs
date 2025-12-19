using UnityEngine;

public interface IInputManager
{
    Vector2 GetMovementInput(); // WASD
    bool GetAttackKeyDown();    // (자동 공격이라 필요 없을 수 있지만 예비용)
    bool GetBombSkillDown();    // J: 폭격
    bool GetSpeedSkillDown();   // K: 가속
    bool GetShieldSkillDown();  // L: 방어
    bool GetInteractDown();     // Space: 대화
}