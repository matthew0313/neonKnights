using UnityEngine;
public class PooledPrefab<T> : MonoBehaviour where T : PooledPrefab<T>
{
    protected virtual int maxPoolSize => 100;
    protected virtual int defaultPoolSize => 0;

    T prefabOrigin = null;
    Pooler<T> pool = null;
    public T Instantiate()
    {
        if (prefabOrigin != null) return prefabOrigin.Instantiate();
        if (pool == null) pool = new Pooler<T>(() =>
        {
            T tmp = Create();
            tmp.OnCreate();
            return tmp;
        }, maxPoolSize, defaultPoolSize);
        T tmp = pool.GetObject();
        tmp.prefabOrigin = this as T;
        tmp.released = false;
        return tmp;
    }
    public bool released { get; private set; } = false;
    public void Release()
    {
        if (prefabOrigin == null || released) return;
        prefabOrigin.pool.ReleaseObject(this as T);
        released = true;
        OnRelease();
    }
    protected virtual T Create()
    {
        return MonoBehaviour.Instantiate((this as T));
    }
    protected virtual void OnCreate() { }
    protected virtual void OnRelease() { }
}