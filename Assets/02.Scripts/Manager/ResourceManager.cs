using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
   
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if(index >= 0)
                name = name.Substring(index + 1);

            GameObject go = GameManager.Instance.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null, Transform spawnPos = null, int count = 0)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Faiiled to Load Prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return GameManager.Instance.Pool.Pop(original, parent, spawnPos, count).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        // Object로 명시함으로써 재귀가 되지 않도록 함
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        // pool push
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            GameManager.Instance.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }

}
