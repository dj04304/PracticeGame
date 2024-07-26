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
        yield return null; // 매 프레임마다 호출되지 않도록 하는데 필요한 경우

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
                        yield break; // 필요 개수 도달 시 종료
                    }

                    float x = _initX + (j * _xSpacing);
                    float z = i * _zSpacing;
                    float y = _currentLayer * _ySpacing; // 레이어에 따라 y 위치 조정

                    Vector3 localPosition = new Vector3(x, y, z);
                    // Instantiate도 pop가능
                    GameObject money = GameManager.Instance.Resource.Instantiate("Pool/Money", null, transform, 1);
                    Poolable pool = money.GetComponent<Poolable>();
                    
                    if (pool == null)
                    {
                        Debug.LogError("Failed to spawn Money object.");
                        continue;
                    }

                    Quaternion originalRotation = money.transform.localRotation; // 원래 로컬 회전을 저장
                    money.transform.SetParent(gameObject.transform);
                    money.transform.localPosition = localPosition;
                    money.transform.localRotation = originalRotation; // 원래 로컬 회전을 다시 설정


                    SitMoneyTriggerHandler handler = gameObject.GetComponentInChildren<SitMoneyTriggerHandler>();
                    handler.MoneyList.Add(money);
                    
                    requiredCount--; // 남은 생성 개수 감소
                    _objCount++;
                }
            }
        }

        _isSpawningSitMoney = false;
    }
}
