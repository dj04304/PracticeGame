using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField]
    protected float _duration = 0.2f; // ������ ���� �ð�
    [SerializeField]
    protected float _height = 1f; // ������ ����

    [SerializeField]
    protected float heightIncrement = 0.3f; // ���� ������

    // ���� �� Ÿ�� ����
    protected Vector3 _startPosition;
    protected Vector3 _targetPosition;
    
    //duration���� �ð� ���
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

        // TODO ����, �� �� ���� ���� ī��Ʈ�� �� �� ����
        _croassantCount = _playerController.GetCroassantStackCount();
    }

    protected virtual void Update()
    {
        // ������ �߻�
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

    // ���� ����
    protected abstract Transform GetArrivalTarget();

    // ���� ���� �ൿ
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
