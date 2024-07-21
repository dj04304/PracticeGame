using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class UI_Max : UI_Base
{
    enum Texts
    {
        MaxText,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Texts));
        gameObject.SetActive(false);
    }

    public void MaxCount()
    {
        gameObject.SetActive(true);
    }
}
