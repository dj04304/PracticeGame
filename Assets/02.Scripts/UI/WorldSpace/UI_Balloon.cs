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
        Chair,

    }

    enum Texts
    {
        BreadText,
    }

    private GameObject _pay;
    private GameObject _bread;
    private GameObject _chair;
    private TMP_Text _breadText;


    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        _pay = GetObject((int)GameObjects.Pay);
        _bread = GetObject((int)GameObjects.Bread);
        _chair = GetObject((int)GameObjects.Chair);
        _breadText = GetText((int)Texts.BreadText);

        gameObject.SetActive(false);

        _bread.SetActive(false);
        _pay.SetActive(false);
        _chair.SetActive(false);
    }

    private void OnEnable()
    {
        SetInfo();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            SetInfo();
    }

    private void SetInfo()
    {
        Transform parent = gameObject.transform.parent;

        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
    }

    // UI 활성화 비활성화 함수
    public void SetUIActive( bool isActive, bool bread = false, bool pay = false, bool chair = false)
    {
        gameObject.SetActive(isActive);

        NPCController nPCController = gameObject.transform.parent.GetComponent<NPCController>();

        if (_bread != null)
        {
            _bread.SetActive(bread);
            _breadText.text = $"{nPCController.RanCroassantMaxCount}";
        }

        if ( _pay != null)
        {
            _pay.SetActive(pay);
        }

        if(_chair != null)
        {
            _chair.SetActive(chair);
        }
    }

    public void UpdateText(int requestCount)
    {
        _breadText.text = $"{requestCount}";
    }

}
