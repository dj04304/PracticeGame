using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroassantProjectile : MonoBehaviour
{
    [SerializeField]
    private float _duration = 2f; // 포물선 비행 시간
    [SerializeField]
    private float _height = 5f; // 포물선 높이

    [SerializeField]
    float heightIncrement = 0.3f; // 크로아상 높이

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _timeElapsed;

    private int _croassantCount;

    GameObject _player;
    PlayerController _playerController;

    public void Initialize(Vector3 startPosition, Vector3 targetPosition)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _timeElapsed = 0f;
        transform.position = startPosition;

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerController  = _player.GetComponent<PlayerController>();
        _croassantCount = _playerController.GetCroassantStackCount();
    }

    // 크로아상 발사
    private void Update()
    {
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
            if (_playerController != null)
            {
                Transform breadPosition = _playerController.BreadPosition;
                if (breadPosition != null)
                {
                    transform.SetParent(breadPosition);
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
    }
}
