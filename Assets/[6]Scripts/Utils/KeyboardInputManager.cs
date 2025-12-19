using UnityEngine;

public class KeyboardInputManager : IInputManager
{
    private GameControls controls;

    public KeyboardInputManager()
    {
        controls = new GameControls();
        controls.Player.Enable(); 
    }

    // 인터페이스 구현부

    public Vector2 GetMovementInput()
    {
        return controls.Player.Move.ReadValue<Vector2>();
    }

    // 기본 공격 자동
    public bool GetAttackKeyDown()
    {
        return true;
    }

    // 스킬 J (폭격)
    public bool GetBombSkillDown()
    {
        return controls.Player.Bomb.WasPressedThisFrame();
    }

    // 스킬 K (가속)
    public bool GetSpeedSkillDown()
    {
        return controls.Player.Speed.WasPressedThisFrame();
    }

    // 스킬 L (방어)
    public bool GetShieldSkillDown()
    {
        return controls.Player.Shield.WasPressedThisFrame();
    }

    // 대화 Space
    public bool GetInteractDown()
    {
        return controls.Player.Interact.WasPressedThisFrame();
    }
}