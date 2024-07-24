using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private int _money;

    public int Money 
    { 
        get  { return _money; }
        set  { _money = value; }
    }

    void Start()
    {
        _money = 0;
    }

    public void OnIncome(int price)
    {
        Money += price;
    }

}
