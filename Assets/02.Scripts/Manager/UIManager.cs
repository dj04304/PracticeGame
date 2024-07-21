using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class UIManager
{
    public GameObject ListRoot;

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

        return sceneUI;
    }

    // WorldSpace�� UI (ex ��ǳ�� ��)
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return go.GetOrAddComponent<T>();
    }

    public void Clear()
    {
        _sceneUI = null;
    }
}
