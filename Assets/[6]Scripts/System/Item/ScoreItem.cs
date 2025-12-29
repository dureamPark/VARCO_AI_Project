using UnityEngine;

public class ScoreItem : BaseItem
{
        protected override void ApplyEffect(PlayerStats target)
    {
            //GameManager.Instance.scoreManager.AddScoreItem();
            Debug.Log("Score Up Item Used");
    }
}
