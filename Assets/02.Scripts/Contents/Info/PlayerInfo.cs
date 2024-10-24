using System;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private int _money;

    [SerializeField]
    private int _initMoney;

    public int Money
    {
        get { return _money; }
        set 
        {
            _money = value;
            OnMoneyUpdate.Invoke(_money);
        }
    }

    public Action<int> OnMoneyUpdate;

    public void OnIncome(int price)
    {
        Money += price;
        
    }

    public int InitMoney
    {
        get { return _initMoney; }
        set { _initMoney = value; } 
    }


}
