using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private float fireRate = 0.2f;

    private float lastFireTime;
    private PlayerStats stats;

    private bool isFiring = false;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }
    public void HandleShooting(bool isAttackDown)
    {
        if (isAttackDown)
        {
            isFiring = !isFiring;
            Debug.Log($"자동 공격 상태: {isFiring}");
        }

        if (isFiring)
        {
            if (Time.time >= lastFireTime + fireRate)
            {
                Fire();
                lastFireTime = Time.time;
            }
        }
    }

    private void Fire()
    {
        GameObject bulletObj = ObjectPoolManager.Instance.Spawn(bulletPrefab, transform.position, Quaternion.identity);

        PlayerProjectile projectile = bulletObj.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(Vector2.up, stats.AttackPower);
        }
    }
}