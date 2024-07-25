using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyInfo : MonoBehaviour
{
    [SerializeField]
    private int _price = 1;

    public int Price { get { return _price; } }
}
