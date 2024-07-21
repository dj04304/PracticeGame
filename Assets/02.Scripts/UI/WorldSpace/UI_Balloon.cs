using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Balloon : UI_Base
{
    enum GameObjects
    {
        Pay,
        Bread,
        BreadText,
    }

    private GameObject _pay;
    private GameObject _bread;
    private TMP_Text _breadText;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _pay = GetObject((int)GameObjects.Pay);
        _bread = GetObject((int)GameObjects.Bread);
        _breadText = GetText((int)GameObjects.BreadText);

        gameObject.SetActive(false);
        _bread.SetActive(false);
        _pay.SetActive(false);
    }

    private void Update()
    {
        Transform parent = gameObject.transform.parent;

        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
    }

    // UI 활성화 비활성화 함수
    public void SetUIActive(bool isActive, bool bread = false, bool pay = false)
    {
        gameObject.SetActive(isActive);

        if (bread && _bread != null)
        {
            _bread.SetActive(bread);
        }

        if (pay && _pay != null)
        {
            _pay.SetActive(pay);
        }
    }

}
