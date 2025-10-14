using System;
using System.Collections.Generic;
using UnityEngine;

//풀 키 받는 용도
public interface IPoolObject
{
    void InitName(string name);
}


public class ObjectPool
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialCount, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialCount; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);

            // 씬 전환에도 유지
            UnityEngine.Object.DontDestroyOnLoad(obj);

            if (obj.TryGetComponent<IPoolObject>(out var poolObj))
                poolObj.InitName(prefab.name);

            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        while (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            if (obj == null || obj.activeInHierarchy)
                continue;

            obj.SetActive(true);
            return obj;
        }

        // 없으면 새로 생성
        var newObj = GameObject.Instantiate(prefab, parent);

        // 씬 전환에도 유지
        UnityEngine.Object.DontDestroyOnLoad(newObj);

        if (newObj.TryGetComponent<IPoolObject>(out var poolObj))
            poolObj.InitName(prefab.name);

        return newObj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

// ---------------- Static 풀링 매니저 ----------------
public static class ObjectPoolManager
{
    private static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    private static Transform parent;

    // 초기화 (원하면 부모 Transform 지정 가능)
    public static void Init(Transform poolParent = null)
    {
        parent = poolParent;
    }

    // 풀 생성
    public static void CreatePool(string name, GameObject prefab, int initialCount)
    {
        if (!pools.ContainsKey(name))
            pools.Add(name, new ObjectPool(prefab, initialCount, parent));
    }

    public static void CreatePool(string name, GameObject prefab, int initialCount, Transform parent)
    {
        if (!pools.ContainsKey(name))
            pools.Add(name, new ObjectPool(prefab, initialCount, parent));
    }


    // 풀에서 꺼내기
    public static GameObject Get(string name)
    {
        if (pools.TryGetValue(name, out var pool))
        {
            return pool.Get();
        }
        return null;
    }

    // 풀에 반환
    public static void Return(GameObject obj, string name)
    {
        if (pools.TryGetValue(name, out var pool))
        {
            pool.Return(obj);
        }
        else
        {
            // 풀 없으면 그냥 파괴
            UnityEngine.Object.Destroy(obj);
        }
    }
}
