using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : BaseController
{
    [SerializeField]
    private Transform _firstWaypoint;

    [SerializeField]
    private Transform _targetWaypoint;

    [SerializeField]
    private bool _hasReachedFirstWaypoint = false;

    [SerializeField]
    private int breadCount;

    private bool _moving = false;
    private int _mask = (1 << (int)Define.Layer.Basket | (1 << (int)Define.Layer.CashTable));
    private NavMeshAgent _agent;

    UI_Balloon _ui;

    private Quaternion _lastKnownRotation; // ���������� �ٶ� ȸ���� ����
    private bool _hasSeenTarget = false; // �̹� Ÿ���� �� �������� ����

    public void SetWaypoints(Transform firstWaypoint, Transform targetWaypoint)
    {
        _agent = GetComponent<NavMeshAgent>();

        _firstWaypoint = firstWaypoint;
        _targetWaypoint = targetWaypoint;
        StartMovingToFirstWaypoint();

        // UI
        _ui = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Balloon>(transform);
    }

    public override void Init()
    {
        MoveSpeed = 5.0f;

        WorldObjectType = Define.ObjectsType.NPC;
        _agent.speed = MoveSpeed;
        _agent.stoppingDistance = 1.0f;
    }

    protected override void UpdateIdle()
    {
        if (!_hasSeenTarget) // Ÿ���� �� ���� ������
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f, _mask);
            if (hitColliders.Length > 0)
            {
                Collider closestCollider = null;
                float closestDistance = float.MaxValue;

                foreach (var collider in hitColliders)
                {
                    if (IsInMask(collider.gameObject.layer))
                    {
                        float distance = Vector3.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestCollider = collider;
                        }
                    }
                }

                if (closestCollider != null)
                {
                    //Debug.Log($"Closest Collider: {closestCollider.name}"); // ����� �α�

                    // ���� ����� �繰�� �ٶ󺸰� ��
                    Vector3 direction = (closestCollider.transform.position - transform.position).normalized;

                    // ȸ�� ���
                    _lastKnownRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = _lastKnownRotation;

                    // UI ������Ʈ
                    _ui.SetUIActive(true, bread: true, pay: false);

                    _hasSeenTarget = true; // Ÿ���� �� ���·� ����
                }
            }
        }
        else
        {
            // Ÿ���� �� �Ŀ��� ��� ���� ���� ����
            transform.rotation = _lastKnownRotation;
        }
    }

    private bool IsInMask(int layer)
    {
        return (_mask & (1 << layer)) != 0;
    }

    protected override void UpdateMoving()
    {
        if (_moving)
        {
            if (_agent.remainingDistance < _agent.stoppingDistance && !_agent.pathPending)
            {
                if (!_hasReachedFirstWaypoint)
                {
                    _hasReachedFirstWaypoint = true;
                    StartMovingToTargetWaypoint();
                }
                else
                {
                    // ���� �� Idle ���·� ��ȯ
                    State = Define.State.Idle;
                    _moving = false;
                }
            }
        }
    }

    private void StartMovingToFirstWaypoint()
    {
        if (_firstWaypoint != null)
        {
            if (_agent != null)
            {
                _agent.SetDestination(_firstWaypoint.position);
                State = Define.State.Moving;
                _moving = true;
            }
        }
    }

    private void StartMovingToTargetWaypoint()
    {
        if (_targetWaypoint != null)
        {
            if (_agent != null)
            {
                _agent.SetDestination(_targetWaypoint.position);
                State = Define.State.Moving;
                _moving = true;
            }
        }
    }
}
