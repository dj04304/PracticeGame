using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField]
    protected float _duration = 0.2f; // ������ ���� �ð�
    [SerializeField]
    protected float _height = 1f; // ������ ����

    [SerializeField]
    protected float heightIncrement = 0.3f; // ���� ������

    // ���� �� Ÿ�� ���� �� SetParent���� ��������
    protected Vector3 _startPosition;
    protected Vector3 _targetPosition;
    protected Transform _arrivalTarget;

    //duration���� �ð� ���
    protected float _timeElapsed;

    protected int _croassantCount;

    protected GameObject _player;
    protected PlayerController _playerController;

    protected ArriveType _arriveType;

    private bool _hasArrived;
    private bool _isMax;

    // ���� ����
    protected virtual Transform GetArrivalTarget() { return _arrivalTarget; }
    public bool HasArrived() { return _hasArrived; }

    public virtual void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform arrivalTarget, Define.ArriveType arriveType)
    {
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
            Arrive(_arriveType);
            _hasArrived = true;
        }
    }

    protected virtual void Arrive(ArriveType arrivalType)
    {
        switch (arrivalType)
        {
            case ArriveType.Basket:
                    BasketArrive();
                break;
            case ArriveType.BreadMachine:
                PlayerHandArrive();
                break;
            default:
                DefaultArrive();
                break;
        }
    }

    // �÷��̾� ��
    private void PlayerHandArrive()
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

                if (_croassantCount == _playerController.CroassantMaxCount())
                {
                    _isMax = true;
                    _playerController.MaxUISetActive(_isMax);
                }
            }

        }
    }

    // �ٱ���
    private void BasketArrive()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // �⺻
    private void DefaultArrive()
    {
        Debug.Log("�⺻ ���� ó��");
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

}
