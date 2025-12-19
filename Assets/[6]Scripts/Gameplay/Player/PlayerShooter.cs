using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private float fireRate = 0.2f;

    private float lastFireTime;
    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }
    public void HandleShooting()
    {
        if (Time.time >= lastFireTime + fireRate)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    private void Fire()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        PlayerProjectile projectile = bulletObj.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(Vector2.up, stats.AttackPower);
        }
    }
}