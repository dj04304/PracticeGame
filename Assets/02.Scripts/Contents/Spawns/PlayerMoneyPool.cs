using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoneyPool : MonoBehaviour
{
    private Transform _parentTransform;
    private bool _isSpawning = false; // 중복 생성 방지용 플래그

    // 누적 카운트
    private int _accumulatedCount = 0;

    private int _objCount;

    public int AddAcumulateCount (int value) {  return _accumulatedCount = value; }

    public void SetParentTransform(Transform parentTransform) { _parentTransform = parentTransform; }

    public Action<int> OnDecreaseMoney;

    PlayerController _playerController;
    PlayerInfo _player;
    Collider _collider;
    GameObject _money;
    Poolable _pool;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _playerController = GameManager.Instance.Spawn.GetPlayer().GetComponent<PlayerController>();
        _player = _playerController.GetComponent<PlayerInfo>();
        _collider = _playerController.GetComponent<Collider>();

        // ObjectPooling cash
        _money = GameManager.Instance.Resource.Instantiate("Pool/ProjectileMoney", null, transform, 30);
        _pool = _money.GetComponent<Poolable>();
        GameManager.Instance.Resource.Destroy(_money);
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

        while (_objCount < _accumulatedCount)
        {
            _pool =  GameManager.Instance.Pool.Pop(_money);

            Vector3 startPosition = _collider.bounds.center;
            Vector3 targetPosition = _parentTransform.position;

            MoneyProjectile projectile = _pool.GetComponent<MoneyProjectile>();

            if (projectile != null)
            {
                projectile.Initialize(startPosition, targetPosition, _parentTransform, Define.ArriveType.CashType);
            }

            yield return new WaitUntil(() => projectile.HasArrived());

            _player.Money--;
            _objCount++;

            GameManager.Instance.Pool.Push(_pool);

            OnDecreaseMoney?.Invoke(1);
        }

        _isSpawning = false; // Reset flag once spawning is complete
    }


}


