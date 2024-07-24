using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField]
    protected float _duration = 0.1f; // 포물선 비행 시간
    [SerializeField]
    protected float _height = 0.3f; // 포물선 높이

    [SerializeField]
    protected float heightIncrement = 0.3f; // 높이 증가량

    // 시작 및 타겟 지점 및 SetParent해줄 도착지점
    protected Vector3 _startPosition;
    protected Vector3 _targetPosition;
    protected Transform _arrivalTarget;

    //duration과의 시간 경과
    protected float _timeElapsed;

    protected int _stackCount;

    protected bool _isMax;

    protected GameObject _player;
    protected PlayerController _playerController;

    protected ArriveType _arriveType;

    private bool _hasArrived;

    // 도착 지점
    protected virtual Transform GetArrivalTarget() { return _arrivalTarget; }
    public bool HasArrived() { return _hasArrived; }

    public virtual void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform arrivalTarget, Define.ArriveType arriveType, int stackCount, float duration = 0.1f)
    {
        _duration = duration;
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _arrivalTarget = arrivalTarget;
        transform.position = startPosition;

        _hasArrived = false;
        _isMax = false;
        _timeElapsed = 0f;

        _arriveType = arriveType;

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerController = _player.GetComponent<PlayerController>();

        // TODO 봉투, 돈 등 여러 스택 카운트가 될 수 있음
        _stackCount = stackCount;
    }

    protected virtual void Update()
    {
        // 포물선 발사
        if (_timeElapsed < _duration)
        {
            _timeElapsed += Time.deltaTime;
            float t = _timeElapsed / _duration;
            float height = Mathf.Sin(t * Mathf.PI) * _height;

            Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, t);
            newPosition.y += height;
            transform.position = newPosition;
        }
        else
        {
            Arrive(_arriveType);
            _hasArrived = true;
        }
    }

    protected virtual void Arrive(ArriveType arrivalType)
    {
        switch (arrivalType)
        {
            case ArriveType.NomalType:
                ToZeroArrive();
                break;
            case ArriveType.StackType:
                PlayerHandArrive();
                break;
            case ArriveType.CashType:
                CashTableArrive();
                break;
            case ArriveType.BagType:
                BagToNPCArrive();
                break;
        }
    }

    private void BagToNPCArrive()
    { 
        Transform arrivalTarget = GetArrivalTarget();

        if (arrivalTarget != null)
        {
            transform.SetParent(arrivalTarget);

            // 오브젝트를 부모의 위치와 회전에 맞추어 배치
            transform.localPosition = Vector3.zero;

            transform.localRotation = Quaternion.Euler(0, 90f, 0);
        }
    }

    // 플레이어 손
    protected virtual void PlayerHandArrive()
    {
        Transform arrivalTarget = GetArrivalTarget();
        if (arrivalTarget != null)
        {
            transform.SetParent(arrivalTarget);

            if (_stackCount <= 0)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.localPosition = new Vector3(0, heightIncrement * (_stackCount - 1), 0);
                transform.localRotation = Quaternion.identity;

                if (_stackCount == _playerController.CroassantMaxCount())
                {
                    _isMax = true;
                    _playerController.MaxUISetActive(_isMax);
                }
            }

        }
    }

    protected virtual void CashTableArrive()
    {
        // 포물선 경로가 자연스럽도록 조정
        transform.localPosition = _targetPosition;
        transform.localRotation = Quaternion.identity;
    }


    // 기본
    protected virtual void ToZeroArrive()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }


}
