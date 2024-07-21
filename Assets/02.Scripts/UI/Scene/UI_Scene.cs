using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    public override void Init()
    {
        GameManager.Instance.UI.SetCanvas(gameObject, false);
    }

}
