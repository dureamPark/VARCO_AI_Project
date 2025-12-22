using UnityEngine;

public class KeyboardInputManager : IInputManager
{
    private GameControls controls;

    public KeyboardInputManager()
    {
        controls = new GameControls();
        controls.Player.Enable(); // 입력 활성화
    }


    public Vector2 GetMovementInput()
    {
        return controls.Player.Move.ReadValue<Vector2>();
    }

    // 공격 (Z키)
    public bool GetAttackDown()
    {
        return controls.Player.Attack.WasPressedThisFrame();
    }

    // 스킬 1: 유도 (X키)
    public bool GetFlowStyleDown()
    {
        return controls.Player.FlowStyle.WasPressedThisFrame();
    }

    // 스킬 2: 방벽 (C키)
    public bool GetBarrierKey()
    {
        return controls.Player.Barrier.IsPressed();
    }

    // 스킬 3: 필살기 (Ctrl키)
    public bool GetOverWriteDown()
    {
        return controls.Player.OverWrite.WasPressedThisFrame();
    }

    // 대화 (Space)
    public bool GetInteractDown()
    {
        return controls.Player.Interact.WasPressedThisFrame();
    }
}