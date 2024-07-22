using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class UI_Max : UI_Base
{
    enum Texts
    {
        MaxText,
    }

    private TMP_Text _maxText;

    public override void Init()
    {
        Bind<GameObject>(typeof(Texts));
        _maxText = GetText((int)Texts.MaxText);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetPos();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            SetPos();
        
    }

    private void SetPos()
    {
        Transform parent = gameObject.transform.parent;
        PlayerController playerController = parent.GetComponent<PlayerController>();

        float croassantHeight = playerController.GetCroassantStackCount() * 0.3f;

        transform.position = parent.position + new Vector3(0, croassantHeight + 1.5f, 0);
        transform.rotation = Camera.main.transform.rotation;
    }

    public void MaxCount(bool isMax)
    {
        if(isMax)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
