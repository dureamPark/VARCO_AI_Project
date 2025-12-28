using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject normalBulletPrefab;
    [SerializeField] private GameObject homingBulletPrefab;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletSpacing = 0.4f;

    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private float lastFireTime;
    private PlayerStats stats;
    private bool isFiring = false;
    public bool IsHomingMode { get; private set; } = false;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        audioSource = GetComponent<AudioSource>();
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

        int currentLevel = stats.AttackPower;

        if (currentLevel >= 76)
        {
            CreateBullet(prefabToUse, Vector2.zero, Vector2.up);     
            CreateBullet(prefabToUse, new Vector2(-bulletSpacing, 0), Vector2.up); 
            CreateBullet(prefabToUse, new Vector2(bulletSpacing, 0), Vector2.up);  

            Vector2 leftDir = Quaternion.Euler(0, 0, 45f) * Vector2.up;
            Vector2 rightDir = Quaternion.Euler(0, 0, -45f) * Vector2.up;

            CreateBullet(prefabToUse, Vector2.zero, leftDir);
            CreateBullet(prefabToUse, Vector2.zero, rightDir);
        }
        // LV 51 ~ 75: [3갈래 일자] 
        else if (currentLevel >= 51)
        {
            CreateBullet(prefabToUse, Vector2.zero, Vector2.up);        
            CreateBullet(prefabToUse, new Vector2(-bulletSpacing, 0), Vector2.up); 
            CreateBullet(prefabToUse, new Vector2(bulletSpacing, 0), Vector2.up);  
        }
        // LV 26 ~ 50: [2갈래 일자] 
        else if (currentLevel >= 26)
        {
            CreateBullet(prefabToUse, new Vector2(-bulletSpacing / 2f, 0), Vector2.up); 
            CreateBullet(prefabToUse, new Vector2(bulletSpacing / 2f, 0), Vector2.up); 
        }
        // LV 1 ~ 25: [1갈래 일자] 
        else
        {
            CreateBullet(prefabToUse, Vector2.zero, Vector2.up); 
        }

        // PlayerAttack 사운드가 다른 사운드 애셋에 비해 소리가 너무 작아서 안들려서 자체 스피커로 구현
        // AudioEvents.TriggerPlaySFX("PlayerAttack");

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    private void CreateBullet(GameObject prefab, Vector2 offset, Vector2 direction)
    {
        Vector3 spawnPos = transform.position + (Vector3)offset;

        GameObject bulletObj = ObjectPoolManager.Instance.Spawn(prefab, spawnPos, Quaternion.identity);

        PlayerProjectile projectile = bulletObj.GetComponent<PlayerProjectile>();
        if (projectile != null)
        {
            int finalDamage = stats.AttackPower;
            if (IsHomingMode) finalDamage = Mathf.Max(1, finalDamage / 2);

            projectile.Initialize(direction, finalDamage, IsHomingMode);

            if (direction != Vector2.up)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}