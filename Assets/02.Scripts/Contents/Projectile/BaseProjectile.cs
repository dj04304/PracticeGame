using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField]
    protected float _duration = 0.2f; // 포물선 비행 시간
    [SerializeField]
    protected float _height = 1f; // 포물선 높이

    [SerializeField]
    protected float heightIncrement = 0.3f; // 높이 증가량

    // 시작 및 타겟 지점
    protected Vector3 _startPosition;
    protected Vector3 _targetPosition;
    
    //duration과의 시간 경과
    protected float _timeElapsed;

    protected int _croassantCount;

    protected GameObject _player;
    protected PlayerController _playerController;

    public virtual void Initialize(Vector3 startPosition, Vector3 targetPosition)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _timeElapsed = 0f;
        transform.position = startPosition;

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerController = _player.GetComponent<PlayerController>();

        // TODO 봉투, 돈 등 여러 스택 카운트가 될 수 있음
        _croassantCount = _playerController.GetCroassantStackCount();
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
            Arrive();
        }
    }

    // 도착 지점
    protected abstract Transform GetArrivalTarget();

    // 도착 시의 행동
    protected virtual void Arrive()
    {
        Transform arrivalTarget = GetArrivalTarget();
        if (arrivalTarget != null)
        {
            transform.SetParent(arrivalTarget);
            if (_croassantCount <= 0)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.localPosition = new Vector3(0, heightIncrement * (_croassantCount - 1), 0);
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
