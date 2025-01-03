using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PooledPrefab<T> : MonoBehaviour where T : PooledPrefab<T>
{
    [Header("Pooling")]
    [SerializeField] bool groupPooledItems = false;

    Transform poolParent;
    T prefabOrigin = null;
    Pooler<T> pool = null;
    public T Instantiate()
    {
        if (prefabOrigin != null) return prefabOrigin.Instantiate();
        if (pool == null) pool = new Pooler<T>(this as T);
        T tmp = pool.GetObject();
        if (groupPooledItems)
        {
            if (poolParent == null) poolParent = new GameObject(gameObject.name + ": Pool Group").transform;
            tmp.poolParent = poolParent;
        }
        tmp.prefabOrigin = this as T;
        tmp.released = false;
        return tmp;
    }
    bool released = false;
    public void Release()
    {
        if (prefabOrigin == null || released) return;
        if (groupPooledItems)
        {
            transform.SetParent(poolParent);
        }
        prefabOrigin.pool.ReleaseObject(this as T);
        released = true;
        OnRelease();
    }
    protected virtual void OnRelease() { }
}