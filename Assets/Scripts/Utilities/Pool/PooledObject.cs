using UnityEngine;
using DG.Tweening;

public class PooledObject : MonoBehaviour
{
    private GameObjectPool m_pool;

    private IPoolable[] m_poolables;

    private Vector3 m_defaultScale;

    public GameObjectPool Pool { get { return m_pool; } }

    public bool IsInPool { get; private set; }

    internal void Setup(GameObjectPool pool)
    {
        m_pool = pool;
        m_poolables = GetComponents<IPoolable>();
        m_defaultScale = transform.localScale;
    }

    internal void OnSpawn()
    {
        IsInPool = false;

        transform.localScale = m_defaultScale;

        for (int i = 0; i < m_poolables.Length; i++)
        {
            m_poolables[i].OnSpawnFromPool();
        }
    }

    internal void OnRelease()
    {
        IsInPool = true;

        transform.DOKill();
        transform.localScale = m_defaultScale;

        for (int i = 0; i < m_poolables.Length; i++)
        {
            m_poolables[i].OnReturnToPool();
        }
    }
}
