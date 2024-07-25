using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    Obj_Base _obj = null;

    private Dictionary<string, Obj_Base> _managedObjects = new Dictionary<string, Obj_Base>();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@Obj_Root");

            if (root == null)
                root = new GameObject { name = "@Obj_Root" };

            return root;
        }
    }

    public T ShowObj<T>(string name = null) where T : Obj_Base
    {
        // 일반적으로 T를 이름이랑 맞출 예정이기 때문에 string name은 기본값이 null, null일 경우 이름을 맞춰줌
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"Obj/{name}");

        T obj = Util.GetOrAddComponent<T>(go);
        _managedObjects[name] = obj;
        _obj = obj;

        go.transform.SetParent(Root.transform);

        return obj;
    }

    // 오브젝트를 이름으로 가져오는 메서드
    public T GetObj<T>(string name = null) where T : Obj_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (_managedObjects.TryGetValue(name, out Obj_Base obj))
        {
            return obj as T;
        }

        return null;
    }

    // 모든 오브젝트를 타입으로 가져오는 메서드
    public List<T> GetAllObjs<T>() where T : Obj_Base
    {
        List<T> result = new List<T>();

        foreach (var obj in _managedObjects.Values)
        {
            if (obj is T t)
            {
                result.Add(t);
            }
        }

        return result;
    }

}
