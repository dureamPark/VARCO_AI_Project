using UnityEngine;
public class HealthUpItem : MonoBehaviour
{
    [SerializeField]
    private int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                //playerStats.Heal(healAmount);
                
                Debug.Log("Heal Item Used");
            }

            Destroy(gameObject);
        }
    }
}

