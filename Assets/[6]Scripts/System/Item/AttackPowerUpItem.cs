using System.Runtime.Serialization;
using UnityEngine;

public class AttackPowerUpItem : MonoBehaviour
{
    //refactor need.
    //한글 되는지 확인하는 주석
    [SerializeField]
    private int attackPowerUp = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                //playerStats.AttackPowerUp(attackPowerUp);
                
                Debug.Log("Attack Power Up Item Used");
            }

            Destroy(gameObject);
        }
    }
}
