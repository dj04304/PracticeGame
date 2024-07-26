using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Money : UI_Scene
{
    enum Images
    {
        MoneyBg,
        Money,
    }

    enum Texts
    {
        Price,
    }

    private TMP_Text _money;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        _money = GetText((int)Texts.Price);

        PlayerInfo player = GameManager.Instance.Spawn.GetPlayer().GetComponent<PlayerInfo>();
        player.OnMoneyUpdate -= OnMoneyUIUpdate;
        player.OnMoneyUpdate += OnMoneyUIUpdate;
    }

    public void OnMoneyUIUpdate(int money)
    {
        _money.text = $"{money}";
    }

}
