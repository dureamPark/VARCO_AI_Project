using UnityEngine;

public class BombItem : BaseItem
{
    [SerializeField]
    private int bombUp = 1;

    protected override void ApplyEffect(PlayerStats target)
    {
            if (target != null)
            {
                target.IncreaseBomb(bombUp);
                
                Debug.Log("bombUP Item Used");
            }
    }
}
