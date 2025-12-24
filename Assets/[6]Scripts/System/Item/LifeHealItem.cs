using System.Runtime.Serialization;
using UnityEngine;
public class LifeHealItem : BaseItem
{
    [SerializeField]
    private int healAmount = 1;

    protected override void ApplyEffect(PlayerStats target)
    {
            if (target != null)
            {
                target.HealLife(healAmount);
                
                Debug.Log("Heal Item Used");
            }
    }
}

