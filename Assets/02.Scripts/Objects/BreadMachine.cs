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
    private int _createCount = 15;

    //[SerializeField]
    //private int _stackMaxCount = 8;

    [SerializeField]
    private float _stackSpeed = 0.5f;

    [SerializeField]
    private float _createSpeed = 1.5f;

    private Queue<GameObject> _croassantQueue = new Queue<GameObject>();
    
    GameObject _croassant;
    
    PlayerController _playerController;
    Collider _collider;

    #region Coroutine Valid
    private Coroutine _croassantSpawnCo;
    private Coroutine _releaseCroassantCo;
    #endregion

    private int _mask = (1 << (int)Define.Layer.Player);
    private bool _isPlayerInRange = false;
    private bool _isCompleteTutorial = false;
    private bool _canCreateCroassant = true; // 초기 값을 true로 설정하여 시작 시 빵을 생성할 수 있게 설정

    private void Start()
    {
        _playerController = GameManager.Instance.Spawn.GetPlayer().GetComponent<PlayerController>();
        _collider = GetComponent<Collider>();

        // ObjectPooling Bread
        _croassant = GameManager.Instance.Resource.Instantiate("Pool/Croassant", null, _breadSpawnPoint, 15);
        GameManager.Instance.Resource.Destroy(_croassant);

        _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());

        // Particle
        Vector3 smokePos = new Vector3(0, 2.7f, 0);
        GameManager.Instance.Particle.Play("VFX_Smoke", gameObject.transform, smokePos, Define.Particle.Loop);

        Vector3 LightPos = new Vector3(0.2f, 1.9f, -0.527f);
        GameManager.Instance.Particle.Play("VFX_OvenLight", gameObject.transform, LightPos, Define.Particle.Loop);
    }

    #region COROUTINE
    private IEnumerator CroassantSpawnCo()
    {
        while (_currentCount < _createCount)
        {
            yield return new WaitForSeconds(_createSpeed);

            CreateBread();
        }

        _canCreateCroassant = false; // 빵을 더 이상 생성할 수 없는 상태로 설정
        _croassantSpawnCo = null; // 코루틴이 종료되면 변수 초기화
    }

    private IEnumerator ReleaseCroassantCo(PlayerController playerController)
    {
        while (_isPlayerInRange)
        {
            if (_currentCount < _playerController.CroassantMaxCount() && _canCreateCroassant) // 변수 이름 변경
            {
                if (_croassantSpawnCo == null)
                {
                    _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());
                }
            }

            if (_croassantQueue.Count > 0 && playerController.GetCroassantStackCount() < _playerController.CroassantMaxCount())
            {
                ProcessBread(playerController);
            }

            yield return new WaitForSeconds(_stackSpeed);
        }

        _releaseCroassantCo = null; // 코루틴이 종료되면 변수 초기화
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            GameManager.Instance.Tutorial.HandleTriggerEnter(other, _isCompleteTutorial, Define.NextTutorial.Basket);
            _isCompleteTutorial = true;
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
    #endregion
    private void CreateBread()
    {
        if (_currentCount < _createCount)
        {
            Poolable pool = GameManager.Instance.Pool.Pop(_croassant);
            _croassantQueue.Enqueue(pool.gameObject);

            _currentCount++;
        }
        else
        {
            _canCreateCroassant = false; // 빵을 더 이상 생성할 수 없는 상태로 설정
        }
    }

    private void ProcessBread(PlayerController playerController)
    {
        if (playerController.GetCroassantStackCount() >= _playerController.CroassantMaxCount())
            return;

        if (_croassantQueue.Count > 0)
        {
            // Spawn 및 스택 쌓기
            GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.HandCroassant, "Pool/HandCroassant");
            playerController.AddToCroassantStack(obj);

            GameObject croassant = _croassantQueue.Dequeue();
            GameManager.Instance.Resource.Destroy(croassant);

            int numberOfCroassant = playerController.GetCroassantStackCount();


            float baseHeight = 0.3f;
            Vector3 startPosition = playerController.Projectile.position;
            Vector3 targetPosition = playerController.HandPos.position;

            if(numberOfCroassant > 1)
            {
                startPosition.y += baseHeight * numberOfCroassant;
                targetPosition.y += baseHeight *numberOfCroassant;
            }

            CroassantProjectile projectile = obj.GetComponent<CroassantProjectile>();

            if (projectile != null)
            {
                projectile.Initialize(startPosition, targetPosition, playerController.HandPos, Define.ArriveType.StackType, numberOfCroassant);
            }

            _currentCount--;

            if (_currentCount < _createCount && !_canCreateCroassant) // 빵을 더 생성할 수 없는 상태가 아니라면
            {
                _canCreateCroassant = true; // 빵을 생성할 수 있는 상태로 변경
                if (_croassantSpawnCo == null)
                {
                    _croassantSpawnCo = StartCoroutine(CroassantSpawnCo());
                }
            }
        }

    }
}
