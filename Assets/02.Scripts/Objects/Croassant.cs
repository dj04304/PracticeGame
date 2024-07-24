using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Croassant : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f; // 이동 속도

    [SerializeField]
    private float _distance = 1.3f; // 이동할 거리

    [SerializeField]
    private Vector3 _startPosition;

    [SerializeField]
    private Vector3 _endPosition;

    private bool _isMoving = false;

    [SerializeField]
    private float _addForce = 2f; // 목표 도달 후 추가 힘

    [SerializeField]
    private float _waitTime = 1f;


    private Rigidbody _rigidbody;
    // 상태 저장
    private bool hasAppliedForce = false;

    void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _endPosition = _startPosition + new Vector3(0, 0, -_distance);

        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        hasAppliedForce = false;
        _isMoving = true;
    }

    void FixedUpdate()
    {
        if (_isMoving && _rigidbody != null)
        {
            // 속도설정
            Vector3 direction = (_endPosition - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            // 목표 도달시 정지
            if (Vector3.Distance(transform.position, _endPosition) < 0.1f)
            {
                _rigidbody.velocity = Vector3.zero;
                _isMoving = false; // 이동 멈춤
                StartCoroutine(AddForceCo(direction));
            }
        }
    }

    private IEnumerator AddForceCo(Vector3 direction)
    {
        // 대기
        yield return new WaitForSeconds(_waitTime);

        // 추가 힘을 적용합니다.
        if (_rigidbody != null && !hasAppliedForce)
        {
            _rigidbody.AddForce(direction * _addForce, ForceMode.Impulse); // 추가 힘
            hasAppliedForce = true;
        }
    }
}
