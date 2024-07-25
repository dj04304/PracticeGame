using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStackMoney : MonoBehaviour
{
    private Transform _parentTransform; // �θ� Ʈ������

    protected int _rows = 3;
    protected int _columns = 3;

    protected float _initX = -1.75f;
    protected float _xSpacing = 0.75f;
    protected float _ySpacing = 0.2f;
    protected float _zSpacing = 0.5f; // zSpacing�� ����� ����

    private bool _isSpawning = false; // �ߺ� ���� ������ �÷���

    // ���� ī��Ʈ
    protected int _accumulatedCount = 0; // 7
    protected int _objCount = 0;
    protected int _currentLayer = 0;

    public int CurrentLayer
    {
        get { return _currentLayer; }
        set { _currentLayer = value; }
    }


    // NPC�� �����ϴ� �ִ� ��
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
                        _isSpawning = false;
                        yield break; // �ʿ� ���� ���� �� ����
                    }

                    float x = _initX + (j * _xSpacing);
                    float z = i * _zSpacing;
                    float y = _currentLayer * _ySpacing; // ���̾ ���� y ��ġ ����

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
                        Quaternion originalRotation = money.transform.localRotation; // ���� ���� ȸ���� ����
                        money.transform.SetParent(_parentTransform, worldPositionStays: false);
                        money.transform.localPosition = localPosition;
                        money.transform.localRotation = originalRotation; // ���� ���� ȸ���� �ٽ� ����

                        MoneyTriggerHandler handler = _parentTransform.GetComponent<MoneyTriggerHandler>();
                        handler.MoneyList.Add(money);
                    }

                    requiredCount--; // ���� ���� ���� ����
                    _objCount++;
                }
            }
        }

        _isSpawning = false;
    }


}
