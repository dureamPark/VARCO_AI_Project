using System.Runtime.Serialization;
using UnityEngine;

public class AttackPowerUpItem : BaseItem
{
    //refactor need.
    //한글 되는지 확인하는 주석
    [SerializeField]
    private int attackPowerUp = 1;

    protected override void ApplyEffect(PlayerStats target)
    {
            if (target != null)
            {
                target.AttackPowerUp(attackPowerUp);
                
                Debug.Log("Attack Power Up Item Used");
            }
    }
}
