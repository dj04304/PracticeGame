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
        // �Ϲ������� T�� �̸��̶� ���� �����̱� ������ string name�� �⺻���� null, null�� ��� �̸��� ������
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"Obj/{name}");

        T obj = Util.GetOrAddComponent<T>(go);
        _managedObjects[name] = obj;
        _obj = obj;

        go.transform.SetParent(Root.transform);

        return obj;
    }

    // ������Ʈ�� �̸����� �������� �޼���
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

    // ��� ������Ʈ�� Ÿ������ �������� �޼���
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
