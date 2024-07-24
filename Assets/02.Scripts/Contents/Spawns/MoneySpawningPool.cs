using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpawningPool : SpawningPool
{
    private Transform _parentTransform; // 부모 트랜스폼

    private int _rows = 3;
    private int _columns = 3;
    
    private float _initX = -1.75f;
    private float _xSpacing = 0.75f;
    private float _ySpacing = 0.2f;
    private float _zSpacing = 0.5f; // zSpacing을 양수로 수정

    private bool _isSpawning = false; // 중복 생성 방지용 플래그

    // 누적 카운트
    private int _accumulatedCount = 0;

    // NPC 수를 이벤트로 받아서 더해주는 용도
    public void AddMoneyCounter(int value) { _objCount += value; }
    // NPC를 유지하는 최대 수
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
        yield return null; // 매 프레임마다 호출되지 않도록 하는데 필요한 경우

        int totalSpawned = GameManager.Instance.Spawn.GetMoney().Count; // 현재까지 스폰된 돈의 개수
        int requiredCount = _accumulatedCount - totalSpawned; // 추가로 필요한 돈의 개수

        while (requiredCount > 0)
        {
            // 현재 레이어를 계산합니다.
            int currentLayer = totalSpawned / (_rows * _columns);

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
                    float y = currentLayer * _ySpacing;

                    Vector3 localPosition = new Vector3(x, y, z);
                    GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.Money, "Pool/Money", transform);

                    if (_parentTransform != null)
                    {
                        obj.transform.SetParent(_parentTransform, worldPositionStays: false);
                    }

                    obj.transform.localPosition = localPosition;
                    requiredCount--; // 남은 생성 개수 감소
                    totalSpawned++; // 총 스폰된 개수 증가
                }
            }
        }

        _isSpawning = false;
    }
}
