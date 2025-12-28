using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRollSkill : EnemySkillBase
{
    public override void CastSkill(int phase, System.Action onFinished)
    {
        this.onSkillEndCallback = onFinished;
        if (playerTransform == null) FindPlayer();
        onSkillEndCallback?.Invoke();
    }
}
