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

    // Popup이 켜졌을 때 해당 캔버스의 기존 UI와의 sort 우선순위를 정하기 위함
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // overrideSorting -> canvas내의 canvas가 있을 경우, 어떤 값을 가지던 자신의 sortingOrder를 가진다는 뜻
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            // sort를 요청안했다 -> popup이아닌 일반 UI
            canvas.sortingOrder = 0;
        }
    }

    // name == 프리팹 이름
    // T == 스크립트
    // SceneUI
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        // 일반적으로 T를 이름이랑 맞출 예정이기 때문에 string name은 기본값이 null, null일 경우 이름을 맞춰줌
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Instance.Resource.Instantiate($"UI/Scene/{name}");

        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        _uiDic[name] = sceneUI;

        return sceneUI;
    }

    // WorldSpace용 UI (ex 말풍선 등)
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

        // 위치
        if (position.HasValue)
            go.transform.position = position.Value;
        else
            go.transform.position = Vector3.zero; // 기본 위치


        // 회전
        if (rotation.HasValue)
            go.transform.rotation = rotation.Value;
        else
            go.transform.rotation = Quaternion.identity; // 기본 회전
        

        // 스케일 설정
        if (scale.HasValue)
            go.transform.localScale = scale.Value;
        else
            go.transform.localScale = Vector3.one; // 기본 스케일

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
