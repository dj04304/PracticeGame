using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStackMoney : MonoBehaviour
{
    private Transform _parentTransform; // 부모 트랜스폼

    protected int _rows = 3;
    protected int _columns = 3;

    protected float _initX = -1.75f;
    protected float _xSpacing = 0.75f;
    protected float _ySpacing = 0.2f;
    protected float _zSpacing = 0.5f; // zSpacing을 양수로 수정

    private bool _isSpawning = false; // 중복 생성 방지용 플래그

    // 누적 카운트
    protected int _accumulatedCount = 0; // 7
    protected int _objCount = 0;
    protected int _currentLayer = 0;

    public int CurrentLayer
    {
        get { return _currentLayer; }
        set { _currentLayer = value; }
    }


    // NPC를 유지하는 최대 수
    public void SetKeepMoneyCount(int count) { _accumulatedCount = count; }
    public void SetParentTransform(Transform parentTransform) { _parentTransform = parentTransform; }

    protected CashTable _cashTable;
    Poolable _pool;
    private void Start()
    {
        Init();
    }

    protected abstract void Init();

    protected virtual void Update()
    {
        if (_objCount < _accumulatedCount && !_isSpawning)
        {
            StartCoroutine(SpawnMoneyCoroutine());
        }
    }

    private IEnumerator SpawnMoneyCoroutine()
    {
        _isSpawning = true;
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
                        _isSpawning = false;
                        yield break; // 필요 개수 도달 시 종료
                    }

                    float x = _initX + (j * _xSpacing);
                    float z = i * _zSpacing;
                    float y = _currentLayer * _ySpacing; // 레이어에 따라 y 위치 조정

                    Vector3 localPosition = new Vector3(x, y, z);

                    GameObject getMoney = _cashTable.GetMoney;
                    _pool = GameManager.Instance.Pool.Pop(getMoney);
                    GameObject money = _pool.gameObject;
                    if (_pool == null)
                    {
                        Debug.LogError("Failed to spawn Money object.");
                        continue;
                    }

                    if (_parentTransform != null)
                    {
                        Quaternion originalRotation = money.transform.localRotation; // 원래 로컬 회전을 저장
                        money.transform.SetParent(_parentTransform, worldPositionStays: false);
                        money.transform.localPosition = localPosition;
                        money.transform.localRotation = originalRotation; // 원래 로컬 회전을 다시 설정

                        MoneyTriggerHandler handler = _parentTransform.GetComponent<MoneyTriggerHandler>();
                        handler.MoneyList.Add(money);
                    }

                    requiredCount--; // 남은 생성 개수 감소
                    _objCount++;
                }
            }
        }

        _isSpawning = false;
    }


}
