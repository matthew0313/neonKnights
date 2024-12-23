using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PooledPrefab<T> : MonoBehaviour where T : PooledPrefab<T>
{
    T prefabOrigin = null;
    Pooler<T> pool = null;
    public T Instantiate()
    {
        if (prefabOrigin != null) return prefabOrigin.Instantiate();
        if (pool == null) pool = new Pooler<T>(this as T);
        T tmp = pool.GetObject();
        tmp.prefabOrigin = this as T;
        tmp.released = false;
        return tmp;
    }
    bool released = false;
    public void Release()
    {
        if (prefabOrigin == null || released) return;
        prefabOrigin.pool.ReleaseObject(this as T);
        released = true;
        OnRelease();
    }
    protected virtual void OnRelease() { }
}