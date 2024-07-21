using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unkown;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
       Object obj = GameObject.FindObjectOfType(typeof(EventSystem));

        if (obj == null)
            GameManager.Instance.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

    }

    public abstract void Clear();

}
