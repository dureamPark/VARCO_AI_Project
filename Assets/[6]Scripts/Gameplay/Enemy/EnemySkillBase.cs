using UnityEngine;
using System.Collections;

public abstract class EnemySkillBase : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected GameObject commonBulletPrefab;
    [SerializeField] protected GameObject warningPrefab;

    protected Transform playerTransform;
    protected System.Action onSkillEndCallback;

    protected virtual void Start()
    {
        FindPlayer();
    }

    protected void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public abstract void CastSkill(int phase, System.Action onFinished);


    protected void CreateBullet(Vector2 pos, Vector2 dir, float speed, float startDelay, BulletShape shape)
    {
        if (commonBulletPrefab == null) return;
        GameObject go = ObjectPoolManager.Instance.Spawn(commonBulletPrefab, pos, Quaternion.identity);

        EnemyPojectile p = go.GetComponent<EnemyPojectile>();
        if (p != null) p.Initialize(dir, 10, speed, startDelay, shape);
    }

    protected EnemyPojectile CreateBulletAndReturn(Vector2 pos, Vector2 dir, float speed, float startDelay, BulletShape shape)
    {
        if (commonBulletPrefab == null) return null;
        GameObject go = ObjectPoolManager.Instance.Spawn(commonBulletPrefab, pos, Quaternion.identity);
        EnemyPojectile p = go.GetComponent<EnemyPojectile>();
        if (p != null) p.Initialize(dir, 10, speed, startDelay, shape);
        return p;
    }

    protected void ShowWarning(Vector2 pos, float duration, float scale)
    {
        if (warningPrefab == null) return;
        GameObject go = ObjectPoolManager.Instance.Spawn(warningPrefab, pos, Quaternion.identity);
        WarningEffect effect = go.GetComponent<WarningEffect>();
        if (effect != null)
        {
            effect.SetOriginPrefab(warningPrefab);
            effect.Initialize(duration, scale);
        }
    }
}