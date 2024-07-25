using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class UIManager
{
    public GameObject ListRoot;

    private Dictionary<string, UI_Base> _uiDic = new Dictionary<string, UI_Base>();

    int _order = 10;
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");

            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    // Popup�� ������ �� �ش� ĵ������ ���� UI���� sort �켱������ ���ϱ� ����
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // overrideSorting -> canvas���� canvas�� ���� ���, � ���� ������ �ڽ��� sortingOrder�� �����ٴ� ��
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            // sort�� ��û���ߴ� -> popup�̾ƴ� �Ϲ� UI
            canvas.sortingOrder = 0;
        }
    }

    // name == ������ �̸�
    // T == ��ũ��Ʈ
    // SceneUI
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        // �Ϲ������� T�� �̸��̶� ���� �����̱� ������ string name�� �⺻���� null, null�� ��� �̸��� ������
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"UI/Scene/{name}");

        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        _uiDic[name] = sceneUI;

        return sceneUI;
    }

    // WorldSpace�� UI (ex ��ǳ�� ��)
    public T MakeWorldSpaceUI<T>(Transform parent = null,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null,
        string name = null
        ) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        // ��ġ
        if (position.HasValue)
            go.transform.position = position.Value;
        else
            go.transform.position = Vector3.zero; // �⺻ ��ġ


        // ȸ��
        if (rotation.HasValue)
            go.transform.rotation = rotation.Value;
        else
            go.transform.rotation = Quaternion.identity; // �⺻ ȸ��
        

        // ������ ����
        if (scale.HasValue)
            go.transform.localScale = scale.Value;
        else
            go.transform.localScale = Vector3.one; // �⺻ ������

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        T component = go.GetOrAddComponent<T>();

        _uiDic[name] = component;

        return component;
    }

    public T GetUI<T>(string name = null) where T : UI_Base
    {
        if(name == null)
            name = typeof(T).Name;

        if (_uiDic.TryGetValue(name, out UI_Base uiBase) == false)
            return null;
        
        
        return uiBase as T;
    }


    public void Clear()
    {
        foreach (var ui in _uiDic.Values)
        {
            if (ui != null)
            {
                GameObject.Destroy(ui.gameObject);
            }
        }

        _uiDic.Clear();
        _sceneUI = null;
    }
}
