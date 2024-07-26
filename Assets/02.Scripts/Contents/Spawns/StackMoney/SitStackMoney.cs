using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitStackMoney : BaseStackMoney
{
    private bool _isSpawningSitMoney = false;

    protected override void Init()
    {
        _initX = 0f;
        _xSpacing = -0.35f;
        _ySpacing = 0.2f;
        _zSpacing = -0.7f; 
    }

    protected override void Update()
    {
        if (_objCount < _accumulatedCount && !_isSpawningSitMoney)
        {
            StartCoroutine(SpawnSitMoneyCoroutine());
        }
    }

    private IEnumerator SpawnSitMoneyCoroutine()
    {
        _isSpawningSitMoney = true;
        yield return null; // �� �����Ӹ��� ȣ����� �ʵ��� �ϴµ� �ʿ��� ���

        int requiredCount = _accumulatedCount - _objCount;

        while (requiredCount > 0)
        {
            _currentLayer = _objCount / (_rows * _columns);
            Debug.Log(_currentLayer);

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (requiredCount <= 0)
                    {
                        _isSpawningSitMoney = false;
                        yield break; // �ʿ� ���� ���� �� ����
                    }

                    float x = _initX + (j * _xSpacing);
                    float z = i * _zSpacing;
                    float y = _currentLayer * _ySpacing; // ���̾ ���� y ��ġ ����

                    Vector3 localPosition = new Vector3(x, y, z);
                    // Instantiate�� pop����
                    GameObject money = GameManager.Instance.Resource.Instantiate("Pool/Money", null, transform, 1);
                    Poolable pool = money.GetComponent<Poolable>();
                    
                    if (pool == null)
                    {
                        Debug.LogError("Failed to spawn Money object.");
                        continue;
                    }

                    Quaternion originalRotation = money.transform.localRotation; // ���� ���� ȸ���� ����
                    money.transform.SetParent(gameObject.transform);
                    money.transform.localPosition = localPosition;
                    money.transform.localRotation = originalRotation; // ���� ���� ȸ���� �ٽ� ����


                    SitMoneyTriggerHandler handler = gameObject.GetComponentInChildren<SitMoneyTriggerHandler>();
                    handler.MoneyList.Add(money);
                    
                    requiredCount--; // ���� ���� ���� ����
                    _objCount++;
                }
            }
        }

        _isSpawningSitMoney = false;
    }
}
