using System.Collections.Generic;
using UnityEngine;
public class GameObjectPool
{
    private readonly GameObject m_prefab;

    private readonly Transform m_root;

    private readonly Stack<PooledObject> m_free = new Stack<PooledObject>();

    public string Key { get; private set; }

    public int CountInPool { get { return m_free.Count; } }

    public int CountCreated { get; private set; }

    public GameObjectPool(string key, GameObject prefab, Transform root)
    {
        Key = key;
        m_prefab = prefab;
        m_root = root;
    }

    public Transform Spawn()
    {
        PooledObject instance = null;

        while (instance == null && m_free.Count > 0)
        {
            instance = m_free.Pop();
        }

        if (instance == null)
        {
            instance = Create();
        }

        Transform view = instance.transform;
        view.SetParent(null);
        view.gameObject.SetActive(true);

        instance.OnSpawn();

        return view;
    }

    public void Release(PooledObject instance)
    {
        if (instance == null || instance.IsInPool) return;

        instance.OnRelease();

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(m_root);

        m_free.Push(instance);
    }

    public void Prewarm(int count)
    {
        while (CountCreated < count)
        {
            Release(Create());
        }
    }

    public void Clear()
    {
        while (m_free.Count > 0)
        {
            PooledObject instance = m_free.Pop();
            if (instance == null) continue;

            Object.Destroy(instance.gameObject);
        }

        CountCreated = 0;
    }

    private PooledObject Create()
    {
        GameObject go = Object.Instantiate(m_prefab);

        PooledObject instance;
        if (!go.TryGetComponent(out instance))
        {
            instance = go.AddComponent<PooledObject>();
        }

        instance.Setup(this);

        CountCreated++;

        return instance;
    }
}
