using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InstanceObjectPool : System.IDisposable
{
    class Pool
    {
        public Stack<GameObject> released = new Stack<GameObject>();
    }

    Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();

    GameObject orphanParent;

    string name;

    public InstanceObjectPool(string name)
    {
        this.name = name;
    }

    public void Dispose()
    {
        pools.Clear();
        orphanParent.SafeDestroyImmediate();
        orphanParent = null;
    }

    Pool GetPool(GameObject instance)
    {
        if (!pools.TryGetValue(instance, out var pool))
            pool = pools[instance] = new Pool();
        return pool;
    }

    public GameObject AcquireInstance(GameObject prefab, Transform parent = null, bool active = true)
    {
        Pool pool = GetPool(prefab);
        if (pool.released.TryPop(out var instance))
        {
            instance.SetActive(active);
        }
        else
        {
            instance = prefab.InstantiatePrefab(active);
            pools[instance] = pool;
        }
        instance.transform.SetParent(parent, worldPositionStays: false);
        return instance;
    }

    public void ReleaseInstance(GameObject instance)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(instance);
            return;
        }
#endif
        if (!orphanParent)
        {
            orphanParent = new GameObject(name);
            orphanParent.SetActive(false);
#if UNITY_EDITOR
            orphanParent.hideFlags = HideFlags.DontSave;
#endif
            Object.DontDestroyOnLoad(orphanParent);
        }
        Debug.Assert(instance.transform.parent != orphanParent.transform, $"{instance} is being released to pool twice. This will cause major errors", instance);
        Pool pool = GetPool(instance);
        pool.released.Push(instance);
        instance.transform.SetParent(orphanParent.transform, worldPositionStays: false);
    }
}