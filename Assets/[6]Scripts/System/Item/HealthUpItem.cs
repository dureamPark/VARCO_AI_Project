using UnityEngine;
public class HealthUpItem : BaseItem
{
    [SerializeField]
    private int healAmount = 1;

    protected override void ApplyEffect(PlayerStats target)
    {
            if (target != null)
            {
                target.Heal(healAmount);
                
                Debug.Log("Heal Item Used");
            }
    }
}

