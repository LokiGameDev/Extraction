using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    private Queue<T> pool = new Queue<T>();

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject()
    {
        T obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);

        return obj;
    }

    public T Get()
    {
        if (pool.Count == 0)
            CreateObject();

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = Get();

        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);

        if (parent != null)
            obj.transform.SetParent(parent);

        pool.Enqueue(obj);
    }
}