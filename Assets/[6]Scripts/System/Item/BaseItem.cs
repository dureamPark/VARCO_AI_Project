using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
                ApplyEffect(playerStats);
                
                Debug.Log($"{gameObject.name} Item Used");
                Destroy(gameObject);
            }
        }
    }

    protected abstract void ApplyEffect(PlayerStats target);
}
