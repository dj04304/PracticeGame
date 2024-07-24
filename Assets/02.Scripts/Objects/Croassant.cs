using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Croassant : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f; // �̵� �ӵ�

    [SerializeField]
    private float _distance = 1.3f; // �̵��� �Ÿ�

    [SerializeField]
    private Vector3 _startPosition;

    [SerializeField]
    private Vector3 _endPosition;

    private bool _isMoving = false;

    [SerializeField]
    private float _addForce = 2f; // ��ǥ ���� �� �߰� ��

    [SerializeField]
    private float _waitTime = 1f;


    private Rigidbody _rigidbody;
    // ���� ����
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
            // �ӵ�����
            Vector3 direction = (_endPosition - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            // ��ǥ ���޽� ����
            if (Vector3.Distance(transform.position, _endPosition) < 0.1f)
            {
                _rigidbody.velocity = Vector3.zero;
                _isMoving = false; // �̵� ����
                StartCoroutine(AddForceCo(direction));
            }
        }
    }

    private IEnumerator AddForceCo(Vector3 direction)
    {
        // ���
        yield return new WaitForSeconds(_waitTime);

        // �߰� ���� �����մϴ�.
        if (_rigidbody != null && !hasAppliedForce)
        {
            _rigidbody.AddForce(direction * _addForce, ForceMode.Impulse); // �߰� ��
            hasAppliedForce = true;
        }
    }
}
