using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    Obj_Base _obj = null;

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

        T Obj = Util.GetOrAddComponent<T>(go);
        _obj = Obj;

        go.transform.SetParent(Root.transform);

        return Obj;
    }

}
