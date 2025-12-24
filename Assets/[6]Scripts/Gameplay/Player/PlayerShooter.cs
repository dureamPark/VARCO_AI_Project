using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject normalBulletPrefab;
    [SerializeField] private GameObject homingBulletPrefab;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.2f;

    private float lastFireTime;
    private PlayerStats stats;
    private bool isFiring = false;
    public bool IsHomingMode { get; private set; } = false;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void HandleShooting(bool isAttackDown)
    {
        if (isAttackDown) isFiring = !isFiring;

        if (isFiring && Time.time >= lastFireTime + fireRate)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    public void SetHomingMode(bool active)
    {
        IsHomingMode = active;
        Debug.Log($"모드 변경: {IsHomingMode}");
    }

    private void Fire()
    {
        // 모드에 따라 프리팹 고르기
        GameObject prefabToUse = IsHomingMode ? homingBulletPrefab : normalBulletPrefab;
        if (prefabToUse == null) return;

        GameObject bulletObj = ObjectPoolManager.Instance.Spawn(prefabToUse, transform.position, Quaternion.identity);

        PlayerProjectile projectile = bulletObj.GetComponent<PlayerProjectile>();
        if (projectile != null)
        {

            int finalDamage = stats.AttackPower;
            if (IsHomingMode) finalDamage = Mathf.Max(1, finalDamage / 2);

            // 총알 초기화
            projectile.Initialize(Vector2.up, finalDamage, IsHomingMode);
        }
    }
}