using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreadMachine : MonoBehaviour
{
    [SerializeField]
    private Transform _breadSpawnPoint;

    [SerializeField]
    private int _currentCount = 0;

    [SerializeField]
    private int _maxCount = 10;

    [SerializeField]
    private int _stackMaxCount = 8;

    [SerializeField]
    private float _stackSpeed = 0.5f;

    [SerializeField]
    private float _createSpeed = 1.5f;

    private Queue<GameObject> _croassantQueue = new Queue<GameObject>();
    private GameObject _croassant;

    private Collider _collider;

    #region Coroutine Valid
    private Coroutine _croassantSpawnCo;
    private Coroutine _releaseCroassantCo;
    #endregion

    private int _mask = (1 << (int)Define.Layer.Player);
    private bool _isPlayerInRange = false;
    private bool _canCreateCroassant = true; // �ʱ� ���� true�� �����Ͽ� ���� �� ���� ������ �� �ְ� ����

    private void Start()
    {
        _collider = GetComponent<Collider>();

        // ObjectPooling Bread
        _croassant = GameManager.Instance.Resource.Instantiate("Pool/Croassant", null, _breadSpawnPoint, 15);
        GameManager.Instance.Resource.Destroy(_croassant);

        _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());
    }

    private IEnumerator CroassantSpawnCo()
    {
        while (_currentCount < _maxCount)
        {
            yield return new WaitForSeconds(_createSpeed);

            CreateBread();
        }

        _canCreateCroassant = false; // ���� �� �̻� ������ �� ���� ���·� ����
        _croassantSpawnCo = null; // �ڷ�ƾ�� ����Ǹ� ���� �ʱ�ȭ
    }

    private IEnumerator ReleaseCroassantCo(PlayerController playerController)
    {
        while (_isPlayerInRange)
        {
            if (_currentCount < _stackMaxCount && _canCreateCroassant) // ���� �̸� ����
            {
                // Create new bread if necessary
                if (_croassantSpawnCo == null)
                {
                    _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());
                }
            }

            if (_croassantQueue.Count > 0 && playerController.GetCroassantStackCount() < _stackMaxCount)
            {
                ProcessBread(playerController);
            }

            yield return new WaitForSeconds(_stackSpeed);
        }

        _releaseCroassantCo = null; // �ڷ�ƾ�� ����Ǹ� ���� �ʱ�ȭ
    }

    private void CreateBread()
    {
        if (_currentCount < _maxCount)
        {
            Poolable pool = GameManager.Instance.Pool.Pop(_croassant);
            _croassantQueue.Enqueue(pool.gameObject);

            _currentCount++;
        }
        else
        {
            _canCreateCroassant = false; // ���� �� �̻� ������ �� ���� ���·� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            if (!_isPlayerInRange)
            {
                _isPlayerInRange = true;

                PlayerController playerController = other.GetComponent<PlayerController>();
                playerController.SetInIdleTrigger(true);

                if (_releaseCroassantCo == null)
                {
                    _releaseCroassantCo = StartCoroutine(ReleaseCroassantCo(playerController));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            if (_isPlayerInRange)
            {
                _isPlayerInRange = false;

                PlayerController playerController = other.GetComponent<PlayerController>();
                playerController.SetInIdleTrigger(false);

                if (_releaseCroassantCo != null)
                {
                    StopCoroutine(_releaseCroassantCo);
                    _releaseCroassantCo = null;
                }
            }
        }
    }

    private void ProcessBread(PlayerController playerController)
    {
        if (playerController.GetCroassantStackCount() >= _stackMaxCount)
            return;

        if (_croassantQueue.Count > 0)
        {
            // Spawn�� ���ýױ�
            GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.HandCroassant, "Pool/HandCroassant");
            playerController.AddToCroassantStack(obj);

            GameObject bread = _croassantQueue.Dequeue();
            GameManager.Instance.Resource.Destroy(bread);

            int numberOfBreads = playerController.GetCroassantStackCount();
            Debug.Log(numberOfBreads);

            float heightIncrement = 0.3f;
            float baseHeight = 0.3f;
            Vector3 startPosition = playerController.Projectile.position;
            startPosition.y += baseHeight + (heightIncrement * numberOfBreads);

            Vector3 targetPosition = playerController.BreadPosition.position;

            CroassantProjectile projectile = obj.GetComponent<CroassantProjectile>();

            if (projectile != null)
            {
                projectile.Initialize(startPosition, targetPosition);
            }

            _currentCount--;

            if (_currentCount < _maxCount && !_canCreateCroassant) // ���� �� ������ �� ���� ���°� �ƴ϶��
            {
                _canCreateCroassant = true; // ���� ������ �� �ִ� ���·� ����
                if (_croassantSpawnCo == null)
                {
                    _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());
                }
            }
        }
    }
}
