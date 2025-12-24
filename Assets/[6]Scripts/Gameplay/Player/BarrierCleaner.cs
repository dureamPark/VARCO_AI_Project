using UnityEngine;

public class BarrierCleaner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);
        }
    }
}