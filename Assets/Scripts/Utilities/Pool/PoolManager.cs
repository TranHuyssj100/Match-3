using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager m_instance;

    private readonly Dictionary<string, GameObjectPool> m_pools = new Dictionary<string, GameObjectPool>();

    public static PoolManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PoolManager>();
            }

            if (m_instance == null)
            {
                m_instance = new GameObject("PoolManager").AddComponent<PoolManager>();
            }

            return m_instance;
        }
    }


    public static Transform Spawn(string resourcePath)
    {
        GameObjectPool pool = Instance.GetPool(resourcePath);
        if (pool == null) return null;

        return pool.Spawn();
    }

    public static T Spawn<T>(string resourcePath) where T : Component
    {
        Transform view = Spawn(resourcePath);
        if (view == null) return null;

        return view.GetComponent<T>();
    }


    public static void Despawn(Transform view)
    {
        if (view == null) return;

        PooledObject instance;
        if (!view.TryGetComponent(out instance) || instance.Pool == null)
        {
            Destroy(view.gameObject);
            return;
        }

        instance.Pool.Release(instance);
    }

    public static void Prewarm(string resourcePath, int count)
    {
        GameObjectPool pool = Instance.GetPool(resourcePath);
        if (pool == null) return;

        pool.Prewarm(count);
    }

    public static void ClearAll()
    {
        if (m_instance == null) return;

        foreach (var pool in m_instance.m_pools.Values)
        {
            pool.Clear();
        }
    }

    public GameObjectPool GetPool(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) return null;

        GameObjectPool pool;
        if (!m_pools.TryGetValue(resourcePath, out pool))
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                Debug.LogErrorFormat("PoolManager: no prefab found in Resources at '{0}'", resourcePath);
                return null;
            }

            Transform root = new GameObject(prefab.name).transform;
            root.SetParent(transform);

            pool = new GameObjectPool(resourcePath, prefab, root);
            m_pools.Add(resourcePath, pool);
        }

        return pool;
    }

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    private void OnDestroy()
    {
        if (m_instance != this) return;

        m_pools.Clear();
        m_instance = null;
    }
}
