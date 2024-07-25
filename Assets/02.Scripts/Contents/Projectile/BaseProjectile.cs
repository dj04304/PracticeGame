using System.Collections;
using System.Collections.Generic;
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

    protected Vector3 _startPosition;
    protected Vector3 _targetPosition;
    protected Transform _arrivalTarget;

    protected float _timeElapsed;
    protected int _stackCount;
    protected bool _isMax;

    protected GameObject _player;
    protected PlayerController _playerController;

    protected ArriveType _arriveType;
    private bool _hasArrived;

    protected virtual Transform GetArrivalTarget() { return _arrivalTarget; }
    public bool HasArrived() { return _hasArrived; }

    public virtual void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform arrivalTarget, Define.ArriveType arriveType, int stackCount, float duration = 0.1f)
    {
        _duration = duration;
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _arrivalTarget = arrivalTarget;

        _hasArrived = false;
        _isMax = false;
        _timeElapsed = 0f;

        _arriveType = arriveType;

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerController = _player.GetComponent<PlayerController>();

        _stackCount = stackCount;

        transform.position = _startPosition;
        StartCoroutine(ProjectileCoroutine());
    }

    protected virtual IEnumerator ProjectileCoroutine()
    {
        transform.position = _startPosition;

        while (_timeElapsed < _duration)
        {
            _timeElapsed += Time.deltaTime;
            float t = _timeElapsed / _duration;
            float height = Mathf.Sin(t * Mathf.PI) * _height;

            Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, t);
            newPosition.y += height;
            transform.position = newPosition;

            yield return null;
        }

        transform.position = _targetPosition;
        Arrive(_arriveType);
        _hasArrived = true;
    }

    protected virtual void Arrive(ArriveType arrivalType)
    {
        switch (arrivalType)
        {
            case ArriveType.CotainType:
                ToZeroArrive();
                break;
            case ArriveType.StackType:
                PlayerHandArrive();
                break;
            case ArriveType.CashType:
                CashArrive();
                break;
            case ArriveType.BagType:
                BagToNPCArrive();
                break;
            case ArriveType.NomalType:
                NomalType();
                break;
        }
    }

    private void BagToNPCArrive()
    {
        Transform arrivalTarget = GetArrivalTarget();

        if (arrivalTarget != null)
        {
            transform.SetParent(arrivalTarget);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 90f, 0);
        }
    }

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
            GameManager.Instance.Sound.Play("Get_Obj");
        }
    }

    protected virtual void CashArrive()
    {
        transform.localPosition = _targetPosition;
        transform.localRotation = Quaternion.identity;
        GameManager.Instance.Sound.Play("Cost_Money");
    }

    protected virtual void NomalType()
    {
        transform.localPosition = _targetPosition;
        transform.localRotation = Quaternion.identity;
    }

    protected virtual void ToZeroArrive()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GameManager.Instance.Sound.Play("Put_Object");
    }
}