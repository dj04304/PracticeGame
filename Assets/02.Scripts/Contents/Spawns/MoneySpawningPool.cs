using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpawningPool : SpawningPool
{
    private Transform _parentTransform; // �θ� Ʈ������

    private int _rows = 3;
    private int _columns = 3;
    
    private float _initX = -1.75f;
    private float _xSpacing = 0.75f;
    private float _ySpacing = 0.2f;
    private float _zSpacing = 0.5f; // zSpacing�� ����� ����

    private bool _isSpawning = false; // �ߺ� ���� ������ �÷���

    // ���� ī��Ʈ
    private int _accumulatedCount = 0;

    // NPC ���� �̺�Ʈ�� �޾Ƽ� �����ִ� �뵵
    public void AddMoneyCounter(int value) { _objCount += value; }
    // NPC�� �����ϴ� �ִ� ��
    public void SetKeepMoneyCount(int count) { _accumulatedCount = count; }
    public void SetParentTransform(Transform parentTransform) { _parentTransform = parentTransform; }

    public override void Init()
    {
        GameManager.Instance.Spawn.OnMoneySpawnEvent -= AddMoneyCounter;
        GameManager.Instance.Spawn.OnMoneySpawnEvent += AddMoneyCounter;
    }

    private void Update()
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

        int totalSpawned = GameManager.Instance.Spawn.GetMoney().Count; // ������� ������ ���� ����
        int requiredCount = _accumulatedCount - totalSpawned; // �߰��� �ʿ��� ���� ����

        while (requiredCount > 0)
        {
            // ���� ���̾ ����մϴ�.
            int currentLayer = totalSpawned / (_rows * _columns);

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
                    float y = currentLayer * _ySpacing;

                    Vector3 localPosition = new Vector3(x, y, z);
                    GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.Money, "Pool/Money", transform);

                    if (_parentTransform != null)
                    {
                        obj.transform.SetParent(_parentTransform, worldPositionStays: false);
                    }

                    obj.transform.localPosition = localPosition;
                    requiredCount--; // ���� ���� ���� ����
                    totalSpawned++; // �� ������ ���� ����
                }
            }
        }

        _isSpawning = false;
    }
}
