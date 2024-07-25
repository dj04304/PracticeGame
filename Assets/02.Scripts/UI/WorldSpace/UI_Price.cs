using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Price : UI_Base
{
    enum GameObjects
    {
        Price,
    }

    [SerializeField]
    private int _price = 100;

    public int Price 
    {
        get { return _price; }
        set { _price = value; }
    }

    private TMP_Text _priceText;


    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _priceText = GetObject((int)GameObjects.Price)?.GetComponent<TMP_Text>();

        if (_priceText != null)
            _priceText.text = _price.ToString();

    }

}
